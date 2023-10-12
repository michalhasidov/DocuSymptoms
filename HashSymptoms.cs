using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class HashSymptoms
    {
        private string symptom;

        public string Symptom
        {
            get { return symptom; }
            set { symptom = value; }
        }
        private string realSymtom;

        public string RealSymtom
        {
            get { return realSymtom; }
            set { realSymtom = value; }
        }
        private HashSymptoms next;

        public HashSymptoms Next
        {
            get { return next; }
            set { next = value; }
        }

        public HashSymptoms()
        {

        }


    }
}
