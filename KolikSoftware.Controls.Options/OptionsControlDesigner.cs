using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;

namespace KolikSoftware.Controls.Options
{
    public class OptionsControlDesigner : ParentControlDesigner
    {
        public OptionsControlDesigner()
            : base()
        {
        }

        protected override bool GetHitTest(System.Drawing.Point point)
        {
            OptionsControl control = this.Control as OptionsControl;

            bool buttonSelected = control.categoryToolStrip.GetItemAt(control.categoryToolStrip.PointToClient(point)) != null;

            if (buttonSelected || control.GetChildAtPoint(point) != null)
                return true;
            else
                return false;
        }
    }
}
