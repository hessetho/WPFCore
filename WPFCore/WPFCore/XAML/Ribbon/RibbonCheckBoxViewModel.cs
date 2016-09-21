using System;
using System.ComponentModel;
using System.Diagnostics;
using WPFCore.ViewModelSupport;

namespace WPFCore.XAML.Ribbon
{
    public class RibbonCheckBoxViewModel : RibbonItemBase
    {
        public RibbonCheckBoxViewModel(string header, INotifyPropertyChanged source, string path) : base(header) 
        {
            this.Source = source;
            this.Path = path;
            this.ValidateSourceAndPath();

            source.PropertyChanged += (ds, de) =>
                {
                    if (de.PropertyName == this.Path)
                    {
                        this.OnPropertyChanged("IsChecked");
                    }
                };
        }

        public object Source { get; private set; }
        public string Path { get; private set; }

        [DoesNotAffectChangesFlag]
        public bool IsChecked
        {
            get
            {
                this.ValidateSourceAndPath();
                var pi = this.Source.GetType().GetProperty(this.Path);
                return (bool)pi.GetValue(this.Source);
            }

            set
            {
                this.ValidateSourceAndPath();
                var pi = this.Source.GetType().GetProperty(this.Path);
                pi.SetValue(this.Source, value);

                this.OnPropertyChanged("IsChecked");
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void ValidateSourceAndPath()
        {
            var pi = this.Source.GetType().GetProperty(this.Path, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.SetProperty);
            if (pi == null)
                throw new InvalidOperationException();
        }
    }
}
