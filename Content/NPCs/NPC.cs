using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mantodea.Content.NPCs
{
    public class NPC : Entity
    {
        public bool ActionDone;

        public virtual void DoAction()
        {
            DoneAction();
        }

        public virtual void DoneAction()
        {
            ActionDone = true;
        }
    }
}
