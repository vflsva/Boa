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
    public class NeuralNetTrainerParamAttributes : GH_LinkedParamAttributes
    {
        private Boa_TrainerComponent component;
        private NeuralNetAttributes neuralNetAttributes;

        private readonly Pen wirePenSelected = new Pen(Color.Magenta, 3);
        private readonly Pen wirePenUnselected = new Pen(Color.Magenta, 1);
        private readonly Pen gripPen = new Pen(Color.Magenta, 2);
        private bool draggingWire = false;
        private PointF wireStart = PointF.Empty;
        private PointF wireEnd = PointF.Empty;
        private readonly float gripSelectionRadius = 8;
        private PointF grip => new PointF(Pivot.X - Bounds.Width / 2, Pivot.Y);
        private RectangleF gripBounds => new RectangleF(
            Pivot.X - Bounds.Width / 2,
            Pivot.Y - gripSelectionRadius,
            gripSelectionRadius * 2,
            gripSelectionRadius * 2
            );

        private GH_WireTopology wireTopology;

        public override bool HasInputGrip => false;
        //public override bool HasOutputGrip => false;

        public NeuralNetTrainerParamAttributes(IGH_Param parameter, IGH_Attributes parent) : base(parameter, parent)
        //public CustomAttributes(NeuralNetTrainerComponent component) : base(component)
        {
            component = (Boa_TrainerComponent)parent.DocObject;
            //this.component = component;
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            //if (channel.Equals(GH_CanvasChannel.Last))
            //{
            //    RenderBounds(canvas, graphics);
            //    RenderGripBounds(canvas, graphics);
            //}
            if (channel.Equals(GH_CanvasChannel.Wires))
                RenderWire(graphics);
            else
            {
                if (channel.Equals(GH_CanvasChannel.Objects))
                    RenderCustomGrip(canvas, graphics);

                base.Render(canvas, graphics, channel);
            }

            //RenderCustomGrip(canvas, graphics);
        }

        public void RenderWire(Graphics graphics)
        {
            if (component.HasBeenLinked)
            {
                PointF p1, p2, p3, p4;
                p1 = grip;//InputGrip;
                p4 = neuralNetAttributes.WireEnd;
                float diagonal = (float)Math.Sqrt(Math.Pow(p1.X - p4.X, 2) + Math.Pow(p1.Y - p4.Y, 2));
                p2 = new PointF(p1.X - diagonal / 2, p1.Y);
                p3 = new PointF(p4.X, p4.Y + diagonal / 2);

                Pen wirePen;

                if (Selected)
                    wirePen = wirePenSelected;
                else
                {
                    wirePen = wirePenUnselected;
                    //GraphicsPath path = new GraphicsPath(FileStyleUriParser)
                }

                wirePen.EndCap = LineCap.Round;
                graphics.DrawBezier(wirePen, p1, p2, p3, p4);
                //graphics.DrawLine(wirePen, InputGrip, neuralNetAttributes.WireEnd);
            }
        }

        public void RenderCustomGrip(GH_Canvas canvas, Graphics graphics)
        {
            var gripAnchor = new PointF(InputGrip.X - gripSelectionRadius / 2 - 2, InputGrip.Y - gripSelectionRadius / 2);
            var gripSize = new SizeF(gripSelectionRadius, gripSelectionRadius);
            var gripRect = new RectangleF(gripAnchor, gripSize);

            graphics.DrawArc(gripPen, gripRect, 90, 180);

            //graphics.DrawEllipse(gripPen, new RectangleF(gripAnchor, gripSize));
        }

        public void RenderGripBounds(GH_Canvas canvas, Graphics graphics)
        {
            graphics.DrawRectangle(new Pen(Color.Black, 2), gripBounds.X, gripBounds.Y, gripBounds.Width, gripBounds.Height);
        }

        public void RenderBounds(GH_Canvas canvas, Graphics graphics)
        {
            graphics.DrawRectangle(new Pen(Color.Black, 2), Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Left && draggingWire)
            {
                wireEnd = sender.CursorCanvasPosition;

                return GH_ObjectResponse.Ignore;
            }
            else
                return base.RespondToMouseMove(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Left && draggingWire)
            {
                //var docs = Owner.OnPingDocument().Objects;
                foreach (IGH_DocumentObject d in Owner.OnPingDocument().Objects)
                {
                    if (d.GetType() == typeof(NeuralNetAlgorithmComponent) || d.GetType() == typeof(NeuralNetAlgorithmVariableParamComponent))
                    {
                        var nn = (NeuralNetAlgorithmComponent)d;
                        if (d.Attributes.Bounds.Contains(e.CanvasLocation))
                        {
                            //component.HasBeenLinked = true;
                            neuralNetAttributes = (NeuralNetAttributes)nn.Attributes;
                            component.LinkNeuralNetComponent(ref nn);// (NeuralNetAlgorithmComponent)d;
                                                                 //(IGH_Param)Owner.
                                                                 //Owner.AddVolatileData(Owner.VolatileData.get_Path(0), 0, nn.Network);
                            break;
                        }
                    }

                }
                draggingWire = false;
                sender.Cursor = Cursors.WaitCursor;
                return GH_ObjectResponse.Release;
            }
            else
                return base.RespondToMouseUp(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Left && gripBounds.Contains(e.CanvasLocation))
            {
                draggingWire = true;

                //if (wireTopology == null)
                //    wireTopology = new GH_WireTopology();

                sender.Cursor = Cursors.Hand;
                return GH_ObjectResponse.Capture;
            }
            else
                return base.RespondToMouseDown(sender, e);
        }
    }
}
