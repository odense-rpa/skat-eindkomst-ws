"""
Parser for eIndkomst service responses

Migrated from C# Odk.BluePrism.Skat.Parsers.eIndkomstParser
"""
from typing import Dict, Any, List, Optional
import pandas as pd
from datetime import datetime
from zeep.helpers import serialize_object


class EIndkomstParser:
    """
    Parser for eIndkomst service responses

    This class converts SOAP response objects into pandas DataFrames,
    mirroring the functionality of the C# eIndkomstParser class which
    converted responses to DataTable objects.
    """

    def __init__(self, logger=None):
        """
        Initialize parser

        Args:
            logger: Optional logger instance
        """
        self.logger = logger

    def _log_info(self, message: str):
        """Log info message if logger is available"""
        if self.logger:
            if hasattr(self.logger, 'info'):
                self.logger.info(message)
            elif hasattr(self.logger, 'Info'):
                self.logger.Info(message)

    def _log_error(self, message: str):
        """Log error message if logger is available"""
        if self.logger:
            if hasattr(self.logger, 'error'):
                self.logger.error(message)
            elif hasattr(self.logger, 'Error'):
                self.logger.Error(message)

    def parse_result_to_dataframe(self, response: Any) -> pd.DataFrame:
        """
        Parse SOAP response to pandas DataFrame

        This method mirrors the C# method:
        DataTable ParseResultToDataTable(IndkomstOplysningPersonHent_OType result)

        Args:
            response: SOAP response object from zeep

        Returns:
            pandas DataFrame with flattened income information

        Raises:
            Exception: If parsing fails
        """
        try:
            # Convert zeep objects to plain Python dictionaries
            response_dict = serialize_object(response)

            self._log_info("Parsing SOAP response to DataFrame")

            records = []

            # Navigate response structure
            # Note: The exact structure depends on the WSDL schema
            # This is a placeholder that needs adjustment based on actual responses

            if isinstance(response_dict, dict):
                # Extract main response container
                # Typically: IndkomstOplysningPersonHent_O or similar

                # Extract person information
                person_info = self._extract_person_info(response_dict)

                # Extract income records (blanketter/forms)
                blanketter = self._extract_blanketter(response_dict)

                if blanketter:
                    for blanket in blanketter:
                        record = self._parse_blanket(blanket, person_info)
                        records.append(record)
                else:
                    # If no blanketter, still create a record with person info
                    records.append(person_info)

            # Convert to DataFrame
            df = pd.DataFrame(records)

            self._log_info(f"Parsed {len(records)} income records")

            # If empty, return a DataFrame with expected columns
            if df.empty:
                df = pd.DataFrame(columns=[
                    'cpr', 'navn', 'blanket_type', 'indberetning_type',
                    'se_nummer', 'virksomhed_navn', 'periode_fra', 'periode_til'
                ])

            return df

        except Exception as e:
            self._log_error(f"Error parsing response: {str(e)}")
            # Return empty DataFrame on error instead of crashing
            return pd.DataFrame()

    def _extract_person_info(self, response_dict: Dict[str, Any]) -> Dict[str, Any]:
        """
        Extract person information from response

        Args:
            response_dict: Response dictionary

        Returns:
            Dictionary with person information
        """
        person_info = {}

        # Try to extract person data from common locations
        if 'PersonOplysninger' in response_dict:
            person_data = response_dict['PersonOplysninger']
            person_info['cpr'] = person_data.get('CPRNummer', '')
            person_info['navn'] = person_data.get('Navn', '')

        # Try alternative locations
        if 'Person' in response_dict:
            person_data = response_dict['Person']
            person_info['cpr'] = person_data.get('CPRNummer', '')
            person_info['navn'] = person_data.get('Navn', '')

        return person_info

    def _extract_blanketter(self, response_dict: Dict[str, Any]) -> List[Dict[str, Any]]:
        """
        Extract blanketter (forms) from response

        Args:
            response_dict: Response dictionary

        Returns:
            List of blanket dictionaries
        """
        blanketter = []

        # Try to find blanketter in response
        if 'Blanketter' in response_dict:
            blanket_data = response_dict['Blanketter']

            # Handle both single blanket and list of blanketter
            if isinstance(blanket_data, list):
                blanketter = blanket_data
            elif blanket_data is not None:
                blanketter = [blanket_data]

        # Try alternative locations
        if not blanketter and 'IndkomstOplysninger' in response_dict:
            indkomst = response_dict['IndkomstOplysninger']
            if 'Blanket' in indkomst:
                blanket_data = indkomst['Blanket']
                if isinstance(blanket_data, list):
                    blanketter = blanket_data
                elif blanket_data is not None:
                    blanketter = [blanket_data]

        return blanketter

    def _parse_blanket(
        self, 
        blanket: Dict[str, Any], 
        person_info: Dict[str, Any]
    ) -> Dict[str, Any]:
        """
        Parse individual blanket (form) from response

        Args:
            blanket: Blanket dictionary
            person_info: Person information dictionary

        Returns:
            Dictionary with parsed blanket data
        """
        record = {
            'cpr': person_info.get('cpr', ''),
            'navn': person_info.get('navn', ''),
            'blanket_type': blanket.get('BlanketType', ''),
            'indberetning_type': blanket.get('IndberetningType', ''),
            'indberetning_art': blanket.get('IndberetningArt', ''),
            'se_nummer': blanket.get('SENummer', ''),
            'virksomhed_navn': blanket.get('VirksomhedNavn', ''),
            'cvr_nummer': blanket.get('CVRNummer', ''),
            'periode_fra': blanket.get('PeriodeFra', ''),
            'periode_til': blanket.get('PeriodeTil', ''),
        }

        # Extract field codes and values (FeltKoder)
        felter = blanket.get('Felter', [])
        if not isinstance(felter, list):
            felter = [felter] if felter else []

        for felt in felter:
            if felt:
                felt_kode = felt.get('FeltKode', '')
                felt_vaerdi = felt.get('FeltVaerdi', '')
                if felt_kode:
                    record[f'felt_{felt_kode}'] = felt_vaerdi

        # Extract additional common fields
        if 'Aargang' in blanket:
            record['aargang'] = blanket['Aargang']

        if 'Kvartal' in blanket:
            record['kvartal'] = blanket['Kvartal']

        if 'Maaned' in blanket:
            record['maaned'] = blanket['Maaned']

        return record
