using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.MyEventArgs
{
    class NewInputEventArgs : EventArgs
    {
        public Input Input { get; set; }

        public NewInputEventArgs(Input input)
        {
            Input = input;
        }
    }
}
