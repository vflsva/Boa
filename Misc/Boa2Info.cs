using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Boa2
{
    public class Boa2Info : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Boa2";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("c3e5a152-5a42-40b1-8d3d-c2acf9ca2e04");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
