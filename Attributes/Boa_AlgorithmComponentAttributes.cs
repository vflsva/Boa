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
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace Boa
{
    public class Boa_AlgrithmComponentAttributes : GH_ComponentAttributes
    {
        private readonly Pen highlightPen = new Pen(Color.White, 4);
        private readonly Pen outlinePen = new Pen(Color.Gray, 5);

        public Boa_AlgrithmComponentAttributes(Boa_AlgorithmComponent component) : base(component)
        {
        }

        protected override void Layout()
        {
            //// Compute the width of the NickName of the owner (plus some extra padding), 
            //// then make sure we have at least 80 pixels.
            //int width = GH_FontServer.StringWidth(Owner.NickName, GH_FontServer.Standard);
            //width = Math.Max(width + 10, 80);

            //// The height of our object is always 60 pixels
            //int height = 60;

            ////this.

            //// Assign the width and height to the Bounds property.
            //// Also, make sure the Bounds are anchored to the Pivot
            //Bounds = new RectangleF(Pivot, new SizeF(width, height));

            //int paramNameWidth = GH_FontServer.StringWidth(Owner.Params.Input[0].NickName, GH_FontServer.Standard) + 8;

            //var componentBox = new RectangleF(new PointF(Pivot.X + paramNameWidth, Pivot.Y), new SizeF(width - paramNameWidth, height));

            base.Layout();

            //LayoutInputParams(Owner, componentBox);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Wires)
            {
                var component = (Boa_AlgorithmComponent)Owner;

                if (component.Algorithm.IsBase)
                {
                    RectangleF inflatedBounds = new RectangleF(Bounds.Location, Bounds.Size);
                    //inflatedBounds.Inflate(1, 1);
                    GH_Capsule highlightCapsule = GH_Capsule.CreateCapsule(inflatedBounds, GH_Palette.White);
                    var outline = highlightCapsule.OutlineShape;
                    graphics.DrawPath(outlinePen, outline);

                    //inflatedBounds.Inflate(1, 1);
                    highlightCapsule = GH_Capsule.CreateCapsule(inflatedBounds, GH_Palette.White);
                    outline = highlightCapsule.OutlineShape;
                    graphics.DrawPath(highlightPen, outline);


                    //graphics.DrawRectangle(highlightPen, new Rectangle(
                    //    (int)inflatedBounds.X,
                    //    (int)inflatedBounds.Y,
                    //    (int)inflatedBounds.Width,
                    //    (int)inflatedBounds.Height
                    //    )
                    //    );
                }
            }

            if (channel == GH_CanvasChannel.Objects)
            {
                var component = (Boa_AlgorithmComponent)Owner;
                string dimensionString = component.Algorithm.GetDimensionsString();

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;

                RectangleF textRectangle = new RectangleF(Bounds.X, Bounds.Y - 20, Bounds.Width, Bounds.Height);
                textRectangle.Height = 20;

                // Draw the Algorithm dimensions in a Standard Grasshopper font.
                graphics.DrawString(dimensionString, GH_FontServer.StandardBold, Brushes.Black, textRectangle, format);

                format.Dispose();
                format = null;
            }
        }

        private void RenderSpecial(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            // Render all the wires that connect the Owner to all its Sources.
            if (channel == GH_CanvasChannel.Wires)
            {
                RenderIncomingWires(canvas.Painter, Owner.Params.Input, GH_ParamWireDisplay.@default);
                return;
            }

            // Render the parameter capsule and any additional text on top of it.
            if (channel == GH_CanvasChannel.Objects)
            {
                
                // Define the default palette.
                GH_Palette palette = GH_Palette.Blue;

                Color color = Color.DarkSalmon;
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

                //canvas.Select  
                // Create a new Capsule without text or icon.
                GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, palette);

                foreach (IGH_Param p in Owner.Params.Input)
                    capsule.AddInputGrip(p.Attributes.InputGrip.Y);

                foreach (IGH_Param p in Owner.Params.Output)
                    capsule.AddOutputGrip(p.Attributes.OutputGrip.Y);

                //Image icon = Owner.Icon_24x24;
                //var component = (GH_Component)Owner;
                //var icon = component.Ico

                if (usePalette)
                    capsule.Render(graphics, Selected, Owner.Locked, true);
                else
                    capsule.Render(graphics, Color.DarkSalmon);

                //if (Owner.IconDisplayMode == GH_IconDisplayMode.icon)
                //    graphics.DrawIcon(Owner., m_innerBounds);
                // Always dispose of a GH_Capsule when you're done with it.
                capsule.Dispose();
                capsule = null;

                GH_Capsule innerCapsule = GH_Capsule.CreateTextCapsule(m_innerBounds, m_innerBounds, GH_Palette.Grey, Owner.NickName);
                innerCapsule.Render(graphics, Selected, Owner.Locked, true);
                innerCapsule.Dispose();
                innerCapsule = null;

                // Now it's time to draw the text on top of the capsule.
                // First we'll draw the Owner NickName using a standard font and a black brush.
                // We'll also align the NickName in the center of the Bounds.
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;

                // Our entire capsule is 60 pixels high, and we'll draw 
                // three lines of text, each 20 pixels high.
                RectangleF textRectangle = Bounds;
                textRectangle.Height = 20;

                // Draw the NickName in a Standard Grasshopper font.
                var component = (Boa_AlgorithmComponent)Owner;
                graphics.DrawString(component.Algorithm.GetDimensionsString(), GH_FontServer.Standard, Brushes.Black, textRectangle, format);

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
                // Now we need to draw the median and mean information.
                // Adjust the formatting and the layout rectangle.
                //format.Alignment = StringAlignment.Near;
                //textRectangle.Inflate(-5, 0);

                //textRectangle.Y += 20;
                //graphics.DrawString(String.Format("Median: {0}", Owner.MedianValue), _
                //                    GH_FontServer.StandardItalic, Brushes.Black, _
                //                    textRectangle, format);

                //textRectangle.Y += 20;
                //graphics.DrawString(String.Format("Mean: {0:0.00}", Owner.MeanValue), _
                //                    GH_FontServer.StandardItalic, Brushes.Black, _
                //                    textRectangle, format);

                //// Always dispose of any GDI+ object that implement IDisposable.
                format.Dispose();
            }
        }
    }
}
