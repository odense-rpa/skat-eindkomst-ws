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

            # Debug: Print response structure
            if self.logger:
                import json
                self._log_info(f"Response keys: {list(response_dict.keys()) if isinstance(response_dict, dict) else 'Not a dict'}")
                # Log a sample of the response (first 500 chars)
                try:
                    sample = json.dumps(response_dict, indent=2, default=str)[:1000]
                    self._log_info(f"Response sample: {sample}")
                except:
                    self._log_info(f"Response type: {type(response_dict)}")

            records = []

            # Navigate response structure based on actual SKAT response
            # Structure: IndkomstPersonUddata -> IndkomstOplysningPersonSamling -> 
            #           IndkomstOplysningPersonStruktur -> IndkomstOplysningSamling

            if isinstance(response_dict, dict):
                # Try the actual SKAT response structure
                if 'IndkomstPersonUddata' in response_dict:
                    uddata = response_dict['IndkomstPersonUddata']

                    # Navigate to IndkomstOplysningPersonSamling
                    if uddata and 'IndkomstOplysningPersonSamling' in uddata:
                        person_samling = uddata['IndkomstOplysningPersonSamling']

                        # Handle both single and list of persons
                        if not isinstance(person_samling, list):
                            person_samling = [person_samling] if person_samling else []

                        for person_struktur in person_samling:
                            if not person_struktur:
                                continue

                            # Extract person info
                            person_info = {}
                            if 'IndkomstOplysningPersonInddata' in person_struktur:
                                person_inddata = person_struktur['IndkomstOplysningPersonInddata']
                                if person_inddata and 'IndkomstOplysningValg' in person_inddata:
                                    valg = person_inddata['IndkomstOplysningValg']
                                    if valg and 'CPRNummerBNummer' in valg:
                                        person_info['cpr'] = valg.get('CPRNummerBNummer', '')

                            # Navigate to IndkomstOplysningSamling
                            if 'IndkomstOplysningPersonStruktur' in person_struktur:
                                struktur = person_struktur['IndkomstOplysningPersonStruktur']
                                if struktur and 'IndkomstOplysningSamling' in struktur:
                                    oplysning_samling = struktur['IndkomstOplysningSamling']

                                    # Handle both single and list
                                    if not isinstance(oplysning_samling, list):
                                        oplysning_samling = [oplysning_samling] if oplysning_samling else []

                                    for oplysning in oplysning_samling:
                                        if not oplysning:
                                            continue

                                        # Extract employer info
                                        employer_info = {}
                                        if 'IndberetningPligtigVirksomhed' in oplysning:
                                            virksomhed = oplysning['IndberetningPligtigVirksomhed']
                                            if virksomhed:
                                                employer_info['se_nummer'] = virksomhed.get('SENummer', '')
                                                employer_info['cvr_nummer'] = virksomhed.get('CVRNummer', '')
                                                employer_info['virksomhed_navn'] = virksomhed.get('VirksomhedNavn', '')

                                        # Navigate to wage period information
                                        if 'IndkomstLoenPeriodeSamling' in oplysning:
                                            loen_samling = oplysning['IndkomstLoenPeriodeSamling']

                                            # Handle both single and list
                                            if not isinstance(loen_samling, list):
                                                loen_samling = [loen_samling] if loen_samling else []

                                            for loen_periode in loen_samling:
                                                if not loen_periode:
                                                    continue

                                                # Create record combining person, employer, and period info
                                                record = {**person_info, **employer_info}

                                                # Extract period details
                                                if 'LoenPeriodeOplysningStruktur' in loen_periode:
                                                    struktur_data = loen_periode['LoenPeriodeOplysningStruktur']
                                                    if struktur_data:
                                                        record['periode_fra'] = struktur_data.get('LoenPeriodeFra', '')
                                                        record['periode_til'] = struktur_data.get('LoenPeriodeTil', '')
                                                        record['indberetning_type'] = struktur_data.get('IndberetningType', '')
                                                        record['loen_art'] = struktur_data.get('LoenArt', '')
                                                        record['loen_beloeb'] = struktur_data.get('LoenBeloeb', '')

                                                        # Extract additional fields if present
                                                        if 'ArbejdstidLoen' in struktur_data:
                                                            record['arbejdstid_loen'] = struktur_data['ArbejdstidLoen']
                                                        if 'Feriebeloeb' in struktur_data:
                                                            record['feriebeloeb'] = struktur_data['Feriebeloeb']

                                                records.append(record)
                else:
                    # Fallback: try legacy structure
                    person_info = self._extract_person_info(response_dict)
                    blanketter = self._extract_blanketter(response_dict)

                    if blanketter:
                        for blanket in blanketter:
                            record = self._parse_blanket(blanket, person_info)
                            records.append(record)
                    else:
                        # If no data found, create empty record
                        records.append(person_info if person_info else {})

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
