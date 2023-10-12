using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class State
    {

        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string final;

        public string Final
        {
            get { return final; }
            set { final = value; }
        }


        //מילון של מעברים


        private Dictionary<char, int> transition;

        public Dictionary<char, int> Transition
        {
            get { return transition; }
            set { transition = value; }
        }



        //פעולות בונות
        public State() { }

        public State(int id,string final, Dictionary<char, int> transition)
        {
            this.id = id;
            this.transition = transition;
            this.final = final;
        }








    }
}
