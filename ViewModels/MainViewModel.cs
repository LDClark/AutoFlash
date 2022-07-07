using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;
using System.Windows.Input;

namespace AutoRingSIB
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
        private Struct[] _structuresHigh;
        public Struct[] StructuresHigh
        {
            get => _structuresHigh;
            set => Set(ref _structuresHigh, value);
        }
        private Struct[] _structuresMid;
        public Struct[] StructuresMid
        {
            get => _structuresMid;
            set => Set(ref _structuresMid, value);
        }
        private Struct[] _structuresLow;
        public Struct[] StructuresLow
        {
            get => _structuresLow;
            set => Set(ref _structuresLow, value);
        }
        private Struct[] _structuresVeryLow;
        public Struct[] StructuresVeryLow
        {
            get => _structuresVeryLow;
            set => Set(ref _structuresVeryLow, value);
        }
        private Struct[] _structuresRingHigh;
        public Struct[] StructuresRingHigh
        {
            get => _structuresRingHigh;
            set => Set(ref _structuresRingHigh, value);
        }
        private Struct[] _structuresRingMid;
        public Struct[] StructuresRingMid
        {
            get => _structuresRingMid;
            set => Set(ref _structuresRingMid, value);
        }
        private Struct[] _structuresRingLow;
        public Struct[] StructuresRingLow
        {
            get => _structuresRingLow;
            set => Set(ref _structuresRingLow, value);
        }
        private Struct[] _structuresRingVeryLow;
        public Struct[] StructuresRingVeryLow
        {
            get => _structuresRingVeryLow;
            set => Set(ref _structuresRingVeryLow, value);
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
        private Struct _selectedStructurePTVHigh;
        public Struct SelectedStructurePTVHigh
        {
            get => _selectedStructurePTVHigh;
            set => Set(ref _selectedStructurePTVHigh, value);
        }
        private Struct _selectedStructurePTVMid;
        public Struct SelectedStructurePTVMid
        {
            get => _selectedStructurePTVMid;
            set => Set(ref _selectedStructurePTVMid, value);
        }
        private Struct _selectedStructurePTVLow;
        public Struct SelectedStructurePTVLow
        {
            get => _selectedStructurePTVLow;
            set => Set(ref _selectedStructurePTVLow, value);
        }
        private Struct _selectedStructurePTVVeryLow;
        public Struct SelectedStructurePTVVeryLow
        {
            get => _selectedStructurePTVVeryLow;
            set => Set(ref _selectedStructurePTVVeryLow, value);
        }
        private Struct _selectedStructureRingHigh;
        public Struct SelectedStructureRingHigh
        {
            get => _selectedStructureRingHigh;
            set => Set(ref _selectedStructureRingHigh, value);
        }
        private Struct _selectedStructureRingMid;
        public Struct SelectedStructureRingMid
        {
            get => _selectedStructureRingMid;
            set => Set(ref _selectedStructureRingMid, value);
        }
        private Struct _selectedStructureRingLow;
        public Struct SelectedStructureRingLow
        {
            get => _selectedStructureRingLow;
            set => Set(ref _selectedStructureRingLow, value);
        }
        private Struct _selectedStructureRingVeryLow;
        public Struct SelectedStructureRingVeryLow
        {
            get => _selectedStructureRingVeryLow;
            set => Set(ref _selectedStructureRingVeryLow, value);
        }
        public string InnerMargin { get; set; }
        public string OuterMargin { get; set; }
        public ICommand StartCommand => new RelayCommand(Start);
        public ICommand GetStructuresCommand => new RelayCommand(GetStructures);
        public ICommand GetRingsCommand => new RelayCommand(CreateRings);

        private async void Start()
        {
            StructureSets = await _esapiService.GetStructureSetsAsync();         
        }

        private async void GetStructures()
        {
            StructuresHigh = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "TV");
            StructuresMid = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "TV");
            StructuresLow = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "TV");
            StructuresVeryLow = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "TV");
            StructuresRingHigh = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "RingHigh");
            StructuresRingMid = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "RingMid");
            StructuresRingLow = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "RingLow");
            StructuresRingVeryLow = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "RingVeryLow");
        }

        private async void CreateRings()
        {
            string selectedStructureSetId = SelectedStructureSet?.StructureSetId;
            string ptvHighId = SelectedStructurePTVHigh?.StructureId;
            string ptvMidId = SelectedStructurePTVMid?.StructureId;
            string ptvLowId = SelectedStructurePTVLow?.StructureId;
            string ptvVeryLowId = SelectedStructurePTVVeryLow?.StructureId;

            string ringHighId = SelectedStructureRingHigh?.StructureId;
            string ringMidId = SelectedStructureRingMid?.StructureId;
            string ringLowId = SelectedStructureRingLow?.StructureId;
            string ringVeryLowId = SelectedStructureRingVeryLow?.StructureId;

            double innerMargin = Convert.ToDouble(InnerMargin) * 10;
            double outerMargin = Convert.ToDouble(OuterMargin) * 10;

            _dialogService.ShowProgressDialog("Getting ring high...",
                async progress =>
                {
                    if (ringHighId == "<Create new structure>")
                        ringHighId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "RingHigh");
                });

            _dialogService.ShowProgressDialog("Getting ring mid...",
                async progress =>
                {
                    if (ringMidId == "<Create new structure>")
                        ringMidId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "RingMid");
                });

            _dialogService.ShowProgressDialog("Getting ring low...",
                async progress =>
                {
                    if (ringLowId == "<Create new structure>")
                        ringLowId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "RingLow");
                });

            _dialogService.ShowProgressDialog("Getting ring very low...",
                async progress =>
                {
                    if (ringVeryLowId == "<Create new structure>")
                        ringVeryLowId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "RingVeryLow");
                });

            _dialogService.ShowProgressDialog("Adding RingHigh...",
                async progress =>
                {
                    await _esapiService.AddRingAsync(selectedStructureSetId, ptvHighId, ringHighId, innerMargin, outerMargin);                    
                });

            if (ptvMidId != null)
            {
                _dialogService.ShowProgressDialog("Adding RingMid...",
                    async progress =>
                    {
                        await _esapiService.AddRingAsync(selectedStructureSetId, ptvMidId, ringMidId, innerMargin, outerMargin);
                    });
            }

            if (ptvLowId != null)
            {
                _dialogService.ShowProgressDialog("Adding RingLow...",
                async progress =>
                {
                    await _esapiService.AddRingAsync(selectedStructureSetId, ptvLowId, ringLowId, innerMargin, outerMargin);
                });
            }

            if (ptvVeryLowId != null)
            {
                _dialogService.ShowProgressDialog("Adding RingVeryLow...",
                async progress =>
                {
                    await _esapiService.AddRingAsync(selectedStructureSetId, ptvVeryLowId, ringVeryLowId, innerMargin, outerMargin);
                });
            }

            _dialogService.ShowProgressDialog("Cleaning up rings...",
                async progress =>
                {
                    //MessageBox.Show(selectedStructureSetId + "," + ptvHighId + "," + ptvMidId + "," + ptvLowId + "," + ptvVeryLowId + "," + ringHighId + "," + ringMidId + "," + ringLowId + "," + ringVeryLowId);
                    await _esapiService.CleanUpRingsAsync(selectedStructureSetId, ptvHighId, ptvMidId, ptvLowId, ptvVeryLowId, ringHighId, ringMidId, ringLowId, ringVeryLowId);
                    StructuresRingHigh = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "RingHigh");
                    StructuresRingMid = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "RingMid");
                    StructuresRingLow = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "RingLow");
                    StructuresRingVeryLow = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "RingVeryLow");
                });
        }
    }
}
