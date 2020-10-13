using System;
using System.Drawing;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Rhino.Geometry;

namespace Boa
{
    public class Boa_AlgorithmSolverAttributes : GH_ComponentAttributes
    {
        public Boa_AlgorithmSolverAttributes(IGH_Component component) : base(component)
        {
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
        //    LayoutOutputParams(Owner, componentBox);
        //}

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        { 
            // Render the component capsule and any additional text on top of it.
            if (channel == GH_CanvasChannel.Objects)
            {
                // Define the default palette.
                GH_Palette palette = GH_Palette.Blue;

                Color color = Color.LightCoral;
                //Color color = Color.MediumSpringGreen;
                //Color color = Color.DeepPink;
                bool usePalette = false;
                if (Selected)
                    usePalette = true;

                // Adjust color based on the Owner's worst case messaging level.
                switch (Owner.RuntimeMessageLevel)
                {
                    case GH_RuntimeMessageLevel.Warning:
                        palette = GH_Palette.Warning;
                        usePalette = true;
                        break;

                    case GH_RuntimeMessageLevel.Error:
                        palette = GH_Palette.Error;
                        usePalette = true;
                        break;
                }

                // Create a new Capsule without text or icon.
                GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, palette);

                foreach (IGH_Param p in Owner.Params.Input)
                    capsule.AddInputGrip(p.Attributes.InputGrip.Y);

                foreach (IGH_Param p in Owner.Params.Output)
                    capsule.AddOutputGrip(p.Attributes.OutputGrip.Y);


                if (usePalette)
                    capsule.Render(graphics, Selected, Owner.Locked, true);
                else
                    capsule.Render(graphics, color);

                var icon = Owner.Icon_24x24;
                var point = new PointF(
                    m_innerBounds.X,
                    m_innerBounds.Y + m_innerBounds.Height / 2 - icon.Height / 2
                    );

                graphics.DrawImage(icon, point);

                var modifierIcon = Grasshopper.GUI.GH_StandardIcons.FlattenIcon_24x24;

                // Always dispose of a GH_Capsule when you're done with it.
                capsule.Dispose();
                capsule = null;

                // Now it's time to draw the text on top of the capsule.
                // First we'll draw the Owner NickName using a standard font and a black brush.
                // We'll also align the NickName in the center of the Bounds.
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;

                //Our entire capsule is 60 pixels high, and we'll draw 
                // three lines of text, each 20 pixels high.
                RectangleF textRectangle = Bounds;
                textRectangle.Height = 20;

                format.Alignment = StringAlignment.Far;
                foreach (IGH_Param p in Owner.Params.Input)
                {
                    textRectangle = p.Attributes.Bounds;
                    graphics.DrawString(p.NickName, GH_FontServer.Standard, Brushes.Black, textRectangle, format);
                }

                format.Alignment = StringAlignment.Near;
                foreach (IGH_Param p in Owner.Params.Output)
                {
                    textRectangle = p.Attributes.Bounds;
                    graphics.DrawString(p.NickName, GH_FontServer.Standard, Brushes.Black, textRectangle, format);
                }

                // Always dispose of any GDI+ object that implement IDisposable.
                format.Dispose();

                
                ////we should render this component's parameters after rendering the component
                //foreach (IGH_Param p in Owner.Params.Input)
                //    p.Attributes.RenderToCanvas(canvas, channel);
            }
            else
            {
                base.Render(canvas, graphics, channel);
            }
        }
    }
}
