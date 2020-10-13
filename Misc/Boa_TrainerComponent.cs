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
    public abstract class Boa_TrainerComponent : GH_Component
    {
        private bool hasBeenLinked = false;

        public bool HasBeenLinked {
            get { return hasBeenLinked; }
            private set { hasBeenLinked = value; }
        }
        public NeuralNetAlgorithmComponent neuralNetComponent;
        private Form window;

        public Boa_TrainerComponent(string name, string nickname, string description, string category, string subcategory)
            : base(name, nickname, description, category, subcategory)
        {
            NewInstanceGuid();
            window = null;
        }

        public Boa_TrainerComponent(string name, string nickname, string description)
            : base(name, nickname, description, "Boa", "Machine Learning")
        {
            NewInstanceGuid();
            window = null;
        }

        public void LinkNeuralNetComponent(ref NeuralNetAlgorithmComponent nnToLink)
        {
            neuralNetComponent = nnToLink;
            //trainer = new BackpropagationTrainer(neuralNet, type, learningRate);
            //SetNetworkToParam();
            //neuralNet = nnToLink.Network;
            hasBeenLinked = true;
            neuralNetComponent.ResetNetwork();
            //AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Neural Network Linked " + neuralNetComponent.ComponentGuid.ToString());
        }

        protected void SetNetworkToParam()
        {
            //var DA = 
            if (neuralNetComponent != null)
                Params.Input[0].AddVolatileData(Params.Input[0].VolatileData.get_Path(0), 0, neuralNetComponent.Network);
            else
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Trainer has not been linked to a neural network.");
        }

        protected NeuralNetwork GetNetwork(out bool successful)
        {
            if (neuralNetComponent != null)
            {
                successful = true;
                return neuralNetComponent.Network;
            }
            else
            {
                successful = false;
                return new NeuralNetwork();
            }
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Open Editor", OpenEditorClicked);
            Menu_AppendItem(menu, "Train", TrainNetworkClicked);
        }

        public void TrainNetworkClicked(Object sender, EventArgs e)
        {
            ExpireSolution(true);
        }

        public void OpenEditorClicked(Object sender, EventArgs e)
        {
            DisplayForm();
        }

        public void DisplayForm()
        {
            var owner = Grasshopper.Instances.DocumentEditor;

            if (window == null || window.IsDisposed)
            {
                if (!HasBeenLinked)
                    window = new TrainerWindow();
                else
                    window = new TrainerWindow(this);// (this) { StartPosition = FormStartPosition.Manual };

                GH_WindowsFormUtil.CenterFormOnWindow(window, owner, true);
                owner.FormShepard.RegisterForm(window);

            }
            window.Show(owner);
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Window opened");
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("ec29688c-2cbc-4210-9bc1-a278123a236a"); }
        }
    }
}
