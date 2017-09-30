using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFCore.Data.FlexData;
using WPFCore.Data.StructuredDataReader;
using WPFCore.ViewModelSupport;

namespace WPFCoreTest
{
    public class CsvReaderViewModel : ViewModelCore
    {
        private string fileName;
        private FlexTable<StructuredDataRow> rows;

        private DelimitedFileReader reader = new DelimitedFileReader();

        private ICommand commandAnalyseStructure;
        private ICommand commandReadData;

        public CsvReaderViewModel()
        {
            this.reader.ColumnDelimiter = ";";
            this.reader.SkipLeadingRows = 3;
            this.fileName = @"E:\CloudStation\Umgebungsdaten\NetAtmo\Aussen_2016_Q1.csv";
        }

        public string FileName
        {
            get { return this.fileName; }
            set
            {
                this.fileName = value;
                base.OnPropertyChanged("FileName");
            }
        }

        private FlexColumnDefinitionCollection columnDefinitions;
        public FlexColumnDefinitionCollection ColumnDefinitions
        {
            get => columnDefinitions;
            private set
            {
                this.columnDefinitions = value;
                base.OnPropertyChanged("ColumnDefinitions");
            }
        }

        public FlexTable<StructuredDataRow> Rows
        {
            get => rows;
            set
            {
                rows = value;
                base.OnPropertyChanged("Rows");
            }
        }

        #region CommandAnalyseStructure
        public ICommand CommandAnalyseStructure
        {
            get { return commandAnalyseStructure ?? (commandAnalyseStructure = new RelayCommand(AnalyseStructure, CanAnalyseStructure)); }
        }

        private void AnalyseStructure()
        {
            reader.Filename = this.fileName;
            this.ColumnDefinitions = reader.DoAnalyseStructure();
            reader.ColumnDefinitions = this.ColumnDefinitions;
        }

        private bool CanAnalyseStructure()
        {
            return File.Exists(this.fileName);
        }
        #endregion CommandAnalyseStructure

        #region CommandReadData
        public ICommand CommandReadData
        {
            get { return commandReadData ?? (commandReadData = new RelayCommand(ReadData, CanReadData)); }
        }

        private void ReadData()
        {
            this.Rows = this.reader.DoReadData(this.fileName);
        }

        private bool CanReadData()
        {
            return File.Exists(this.fileName) && this.reader.ColumnDefinitions != null;
        }
        #endregion CommandReadData
    }
}
