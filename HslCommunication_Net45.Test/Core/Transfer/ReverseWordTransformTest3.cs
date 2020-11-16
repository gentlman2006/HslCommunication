using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HslCommunication_Net45.Test.Core.Transfer
{
    [TestClass]
    public class ReverseWordTransformTest3 : ReverseWordTransformTest
    {
        public ReverseWordTransformTest3( )
        {
            byteTransform.DataFormat = HslCommunication.Core.DataFormat.CDAB;
        }



    }
}
