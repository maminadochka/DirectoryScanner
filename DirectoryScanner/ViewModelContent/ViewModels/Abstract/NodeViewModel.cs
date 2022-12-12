namespace Client.ViewModelContent.ViewModels.Abstract
{//абстрактный класс для файла или директории
    public abstract class NodeViewModel : ViewModelBase
    {
        public long Size
        {
            get { return _size; }
            protected set
            {
                _size = value;
                NotifyPropertyChanged(nameof(Size));
            }
        }

        public double? PercentageSize 
        {
            get { return _percentageSize; }
            protected set
            {
                _percentageSize = value;
                NotifyPropertyChanged(nameof(PercentageSize));
            }
        }

        public string Name
        {
            get { return _name; }
            protected set
            {
                _name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        public string Presentation
        {
            get
            {
                if (PercentageSize != null)
                    return $"{Name}  ({Size} байт, {PercentageSize:F2}%)";
                else
                    return $"{Name}  ({Size} байт)";
            }
        }

        private double? _percentageSize;
        private long _size;
        private string _name;


        public NodeViewModel(string name, long size, long? parentSize)
        {
            Name = name;
            Size = size;

            if (parentSize == null)
                PercentageSize = null;
            else
                PercentageSize = (double)size / parentSize * 100;
        }
    }
}
