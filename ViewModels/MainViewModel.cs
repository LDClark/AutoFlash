using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoFlashIMRT
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEsapiService _esapiService;
        private readonly IDialogService _dialogService;
        public MainViewModel(IEsapiService esapiService, IDialogService dialogService)
        {
            _esapiService = esapiService;
            _dialogService = dialogService;
        }
        private StructSet[] _structureSets;
        public StructSet[] StructureSets
        {
            get => _structureSets;
            set => Set(ref _structureSets, value);
        }
        private StructSet _selectedStructureSet;
        public StructSet SelectedStructureSet
        {
            get => _selectedStructureSet;
            set => Set(ref _selectedStructureSet, value);
        }
        private Struct[] _structuresPTVBreast;
        public Struct[] StructuresPTVBreast
        {
            get => _structuresPTVBreast;
            set => Set(ref _structuresPTVBreast, value);
        }
        private Struct[] _structuresPTVSCV;
        public Struct[] StructuresPTVSCV
        {
            get => _structuresPTVSCV;
            set => Set(ref _structuresPTVSCV, value);
        }
        private Struct[] _structuresPTVAxilla;
        public Struct[] StructuresPTVAxilla
        {
            get => _structuresPTVAxilla;
            set => Set(ref _structuresPTVAxilla, value);
        }
        private Struct[] _structuresPTVIMN;
        public Struct[] StructuresPTVIMN
        {
            get => _structuresPTVIMN;
            set => Set(ref _structuresPTVIMN, value);
        }
        private Struct _selectedStructurePTVBreast;
        public Struct SelectedStructurePTVBreast
        {
            get => _selectedStructurePTVBreast;
            set => Set(ref _selectedStructurePTVBreast, value);
        }
        private Struct _selectedStructurePTVSCV;
        public Struct SelectedStructurePTVSCV
        {
            get => _selectedStructurePTVSCV;
            set => Set(ref _selectedStructurePTVSCV, value);
        }
        private Struct _selectedStructurePTVAxilla;
        public Struct SelectedStructurePTVAxilla
        {
            get => _selectedStructurePTVAxilla;
            set => Set(ref _selectedStructurePTVAxilla, value);
        }
        private Struct _selectedStructurePTVIMN;
        public Struct SelectedStructurePTVIMN
        {
            get => _selectedStructurePTVIMN;
            set => Set(ref _selectedStructurePTVIMN, value);
        }
        private string _selectedLaterality;
        public string SelectedLaterality
        {
            get => _selectedLaterality;
            set => Set(ref _selectedLaterality, value);
        }
        private string _anteriorMargin;
        public string AnteriorMargin
        {
            get => _anteriorMargin;
            set => Set(ref _anteriorMargin, value);
        }
        private string _lateralMargin;
        public string LateralMargin
        {
            get => _lateralMargin;
            set => Set(ref _lateralMargin, value);
        }
        private string _outerMargin100;
        public string OuterMargin100
        {
            get => _outerMargin100;
            set => Set(ref _outerMargin100, value);
        }
        private string _innerMargin100;
        public string InnerMargin100
        {
            get => _innerMargin100;
            set => Set(ref _innerMargin100, value);
        }
        private string _outerMargin50;
        public string OuterMargin50
        {
            get => _outerMargin50;
            set => Set(ref _outerMargin50, value);
        }
        private string _innerMargin50;
        public string InnerMargin50
        {
            get => _innerMargin50;
            set => Set(ref _innerMargin50, value);
        }
        private string _lungOptMargin;
        public string LungOptMargin
        {
            get => _lungOptMargin;
            set => Set(ref _lungOptMargin, value);
        }
        public ICommand StartCommand => new RelayCommand(Start);
        public ICommand GetStructuresCommand => new RelayCommand(GetStructures);
        public ICommand CreateStructuresCommand => new RelayCommand(CreateStructures);

        private async void Start()
        {
            AnteriorMargin = "1";
            LateralMargin = "1";
            OuterMargin100 = "3";
            OuterMargin50 = "5";
            InnerMargin100 = "-0.5";
            InnerMargin50 = "-1.5";
            LungOptMargin = "1.5";
            StructureSets = await _esapiService.GetStructureSetsAsync();           
        }

        private async void GetStructures()
        {
            StructuresPTVBreast = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "PTV");
            StructuresPTVSCV = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "PTV");
            StructuresPTVAxilla = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "PTV");
            StructuresPTVIMN = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "PTV");
        }

        private void CreateStructures()
        {
            string selectedStructureSetId = SelectedStructureSet?.StructureSetId;
            string ptvBreastId = SelectedStructurePTVBreast?.StructureId;
            string ptvSCVId = SelectedStructurePTVSCV?.StructureId;
            string ptvAxillaId = SelectedStructurePTVAxilla?.StructureId;
            string ptvIMNId = SelectedStructurePTVIMN?.StructureId;

            string laterality = SelectedLaterality;

            double anteriorMargin = Convert.ToDouble(AnteriorMargin) * 10;
            double lateralMargin = Convert.ToDouble(LateralMargin) * 10;

            double innerMargin100 = Convert.ToDouble(InnerMargin100) * 10;
            double outerMargin100 = Convert.ToDouble(OuterMargin100) * 10;
            double innerMargin50 = Convert.ToDouble(InnerMargin50) * 10;
            double outerMargin50 = Convert.ToDouble(OuterMargin50) * 10;

            double lungOptMargin = Convert.ToDouble(LungOptMargin) * 10;


            _dialogService.ShowProgressDialog("Creating structures...",
                async progress =>
                {
                    await _esapiService.AddStructuresAsync(selectedStructureSetId, ptvBreastId, ptvSCVId, ptvAxillaId, ptvIMNId, laterality, anteriorMargin, lateralMargin,
                        outerMargin100, innerMargin100, outerMargin50, innerMargin50, lungOptMargin);
                });
        }
    }
}
