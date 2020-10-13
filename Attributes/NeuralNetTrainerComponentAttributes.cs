using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using System.Drawing;
using System.Drawing.Drawing2D;
using static Boa.NeuralNetAlgorithmComponent;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace Boa
{
    public class NeuralNetTrainerComponentAttributes : GH_ComponentAttributes
    {
        public NeuralNetTrainerComponentAttributes(IGH_Component component) : base(component)
        {
        }

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            ((Boa_TrainerComponent)Owner).DisplayForm();
            return GH_ObjectResponse.Handled;
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            //paramAttributes.RenderBounds(canvas, graphics);

            base.Render(canvas, graphics, channel);

            if (channel.Equals(GH_CanvasChannel.Objects))
            {
                

                var paramAttributes = (NeuralNetTrainerParamAttributes)Owner.Params.Input[0].Attributes;
                paramAttributes.RenderCustomGrip(canvas, graphics);
            }
        }

        //protected override void Layout()
        //{
        //    // Compute the width of the NickName of the owner (plus some extra padding), 
        //    // then make sure we have at least 80 pixels.
        //    int width = GH_FontServer.StringWidth(Owner.NickName, GH_FontServer.Standard);
        //    width = Math.Max(width + 10, 80);

        //    // The height of our object is always 60 pixels
        //    int height = 60;

        //    // Assign the width and height to the Bounds property.
        //    // Also, make sure the Bounds are anchored to the Pivot
        //    Bounds = new RectangleF(Pivot, new SizeF(width, height));

        //    int paramNameWidth = GH_FontServer.StringWidth(Owner.Params.Input[0].NickName, GH_FontServer.Standard) + 8;

        //    var componentBox = new RectangleF(new PointF(Pivot.X + paramNameWidth, Pivot.Y), new SizeF(width - paramNameWidth, height));
        //    LayoutInputParams(Owner, componentBox);
        //}
    }
}
