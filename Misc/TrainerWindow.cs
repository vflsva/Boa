using System;
using System.Windows;
using System.Windows.Forms;

namespace Boa
{
    public class TrainerWindow : Form
    {
        //private readonly NeuralNetAlgorithmComponent neuralNetComponent;
        private readonly Boa_TrainerComponent trainerComponent;
        private Button trainButton = new Button() { Text = "Train" };
        //trainButton.
        //ToolStr
        //ToolStripItem trainerDrodownBackpropagationType = new ToolStripItem.ToolStripItemAccessibleObject()
        //ToolStripDropDownButton trainerDropdownButton = new ToolStripDropDownButton()

        //EventHandler trainEvent = new EventHandler()

        public TrainerWindow()
        {
            Text = "Example text";
            //trainButton.
            trainButton.Click += new EventHandler(TrainNetwork);
        }

        public TrainerWindow(Boa_TrainerComponent trainerComponent) : this()
        {
            this.trainerComponent = trainerComponent;
        }

        public void TrainNetwork(object o, EventArgs e)
        {
            if (trainerComponent != null)
            {
                trainerComponent.ExpireSolution(true);
            }
        }
    }
}
