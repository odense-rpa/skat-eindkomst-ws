using dk.skat.eindkomst;
using System.Data;

namespace Odk.BluePrism.Skat.Parsers
{
    public interface IeIndkomstParser
    {
        DataTable ParseResultToDataTable(IndkomstOplysningPersonHent_OType personHentType);
    }
}