using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data
{
    public class Holiday : IComparable<Holiday>
    {
        private bool isFix;
        private DateTime datum;
        private string name;

        public Holiday(bool isFix, DateTime datum, string name)
        {
            this.IsFix = isFix;
            this.Datum = datum;
            this.Name = name;
        }

        /// <summary>
        /// Beschreibung: 
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Beschreibung: 
        /// </summary>
        public DateTime Datum
        {
            get { return datum; }
            set { datum = value; }
        }

        /// <summary>
        /// Beschreibung: 
        /// </summary>
        public bool IsFix
        {
            get { return isFix; }
            set { isFix = value; }
        }

        #region IComparable<Feiertag>

        public int CompareTo(Holiday other)
        {
            return this.datum.Date.CompareTo(other.datum.Date);
        }

        #endregion IComparable<Feiertag>
    }
}
