using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace Tekla.Demo.Console.Services
{
    internal class BeamService
    {
        public bool CreateBeam()
        {
            var point = new Point(0, 0, 0);
            var point2 = new Point(1000, 0, 0);
            var beam = new Beam
            {
                StartPoint = point,
                EndPoint = point2,
                Finish = "PAINT",
                StartPointOffset = new(),
                EndPointOffset = new(),
                CastUnitType = Part.CastUnitTypeEnum.PRECAST,
                Name = "PC-1"
            };
            beam.Profile.ProfileString = "600*200";
            beam.Material.MaterialString = "C30";
            beam.Position.Depth = Position.DepthEnum.BEHIND;
            beam.Position.Rotation = Position.RotationEnum.BELOW;
            beam.Position.Plane = Position.PlaneEnum.MIDDLE;
            //构件编号
            beam.AssemblyNumber = new NumberingSeries("PC", 1);
            //零件编号
            beam.PartNumber = new NumberingSeries("PC-1", 1);

            var result = beam.Insert();
            return result;
        }
    }
}
