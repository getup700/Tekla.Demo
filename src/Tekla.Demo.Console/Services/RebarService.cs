using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Analysis;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace Tekla.Demo.Console.Services
{
    internal class RebarService
    {
        double alongLength = 50;
        double plThickness = 40;

        /// <summary>
        /// 创建箍筋
        /// </summary>
        public void CreateHooping()
        {
            var model = new Model();
            if (new Picker().PickObject(Picker.PickObjectEnum.PICK_ONE_PART) is not Beam beam)
            {
                return;
            }

            #region 工作平面
            var currentPlane = model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            //构件的局部工作平面
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(beam.GetCoordinateSystem()));

            #endregion

            #region 获得梁的外轮廓点
            var minPoint = beam.GetSolid().MinimumPoint;
            var maxPoint = beam.GetSolid().MaximumPoint;
            var minX = Math.Min(minPoint.X, maxPoint.X);
            var minY = Math.Min(minPoint.Y, maxPoint.Y);
            var minZ = Math.Min(minPoint.Z, maxPoint.Z);

            var maxX = Math.Max(minPoint.X, maxPoint.X);
            var maxY = Math.Max(minPoint.Y, maxPoint.Y);
            var maxZ = Math.Max(minPoint.Z, maxPoint.Z);

            var startPoint = beam.StartPoint;
            var endPoint = beam.EndPoint;

            #endregion

            #region 箍筋外形
            var polygon1 = new Polygon();
            var p11 = new Point(minX, minY, minZ);
            var p12 = new Point(minX, minY, maxZ);
            var p13 = new Point(minX, maxY, maxZ);
            var p14 = new Point(minX, maxY, minZ);
            var p15 = p11;
            polygon1.Points = [p11, p12, p13, p14, p15];

            var polygon2 = new Polygon();
            var p21 = new Point(maxX, minY, minZ);
            var p22 = new Point(maxX, minY, maxZ);
            var p23 = new Point(maxX, maxY, maxZ);
            var p24 = new Point(maxX, maxY, minZ);
            var p25 = p21;
            polygon2.Points = [p21, p22, p23, p24, p25];

            var rebarGroup = new RebarGroup();
            rebarGroup.Polygons.Add(polygon1);
            rebarGroup.Polygons.Add(polygon2);
            rebarGroup.RadiusValues.Add(16.0);//一定要是双精度浮点型

            rebarGroup.SpacingType = BaseRebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_EXACT_SPACE_FLEX_AT_BOTH;
            rebarGroup.Spacings.Add(200);

            rebarGroup.ExcludeType = BaseRebarGroup.ExcludeTypeEnum.EXCLUDE_TYPE_NONE;
            rebarGroup.Father = beam;
            rebarGroup.Name = "R";
            rebarGroup.Class = 8;
            rebarGroup.Size = "8";
            rebarGroup.NumberingSeries.Prefix = "R";
            rebarGroup.NumberingSeries.StartNumber = 1;
            rebarGroup.Grade = "HRB400";
            rebarGroup.StartHook.Shape = RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK;
            rebarGroup.StartHook.Angle = 135;
            rebarGroup.StartHook.Length = 40;
            rebarGroup.StartHook.Radius = 16;
            rebarGroup.EndHook.Shape = RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK;
            rebarGroup.EndHook.Angle = 135;
            rebarGroup.EndHook.Length = 40;
            rebarGroup.EndHook.Radius = 16;
            rebarGroup.OnPlaneOffsets.Add(20.0);
            rebarGroup.OnPlaneOffsets.Add(20.0);
            rebarGroup.OnPlaneOffsets.Add(20.0);
            rebarGroup.StartPointOffsetType = Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
            rebarGroup.StartPointOffsetValue = 20.0;
            rebarGroup.EndPointOffsetType = Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
            rebarGroup.EndPointOffsetValue = 20.0;
            rebarGroup.FromPlaneOffset = 20.0;

            rebarGroup.Insert();

            //rebarGroup.Name = "Modified Group 1";
            //rebarGroup.Modify();

            #endregion

            #region 恢复工作平面
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);
            model.CommitChanges();

            #endregion

        }

        /// <summary>
        /// 创建纵筋
        /// </summary>
        public void CreateLongitudinalReinforcement()
        {
            var model = new Model();
            if (new Picker().PickObject(Picker.PickObjectEnum.PICK_ONE_PART) is not Beam beam)
            {
                return;
            }

            #region 工作平面，坐标系设置
            //保存当前工作平面
            var transformationPlane = model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            //新建模型坐标系设置为世界坐标系
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());

            //自定义坐标系
            var startPoint = beam.StartPoint;
            var endPoint = beam.EndPoint;
            //创建局部坐标系
            var origin = startPoint;
            var axisX = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, endPoint.Z = startPoint.Z);
            var axisY = new Vector(0, 0, 1);
            var newCoordinate = new CoordinateSystem(origin, axisX, axisY);
            //设置局部工作平面
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(newCoordinate));

            #endregion



            #region 获取梁轮廓
            //获取梁轮廓
            var minPoint = beam.GetSolid().MinimumPoint;
            var maxPoint = beam.GetSolid().MaximumPoint;
            //???
            var minX = Math.Min(minPoint.X, maxPoint.X);
            var minY = Math.Min(minPoint.Y, maxPoint.Y);
            var minZ = Math.Min(minPoint.Z, maxPoint.Z);

            var maxX = Math.Max(minPoint.X, maxPoint.X);
            var maxY = Math.Max(minPoint.Y, maxPoint.Y);
            var maxZ = Math.Max(minPoint.Z, maxPoint.Z);
            #endregion


            #region 绘制纵筋 1号纵筋

            //创建纵筋外形
            var point3 = new Point(minX + 50, minY + 40, minZ + 40);
            var point4 = new Point(maxX - 50, minY + 40, minZ + 40);

            var point1 = new Point(minX - 600, point3.Y + 50, point3.Z + 50);
            var point2 = new Point(minX, point1.Y, point1.Z);

            var point5 = new Point(maxX, point4.Y + 50, point4.Z + 50);
            var point6 = new Point(maxX + 600, point5.Y, point5.Z);

            var polygon = new Polygon
            {
                Points = [
                    point1, point2, point3, point4, point5,point6
                ]
            };

            //设置钢筋属性
            var singleRebar = new SingleRebar();
            singleRebar.Polygon = polygon;
            singleRebar.Father = beam;
            singleRebar.Name = "R";
            singleRebar.Class = 20;
            singleRebar.Size = "20";
            singleRebar.NumberingSeries = new NumberingSeries("R", 1);
            singleRebar.Grade = "HRB400";
            singleRebar.OnPlaneOffsets = new ArrayList() { 0.0 };

            singleRebar.StartHook.Shape = RebarHookData.RebarHookShapeEnum.NO_HOOK;
            singleRebar.EndHook.Shape = RebarHookData.RebarHookShapeEnum.NO_HOOK;

            singleRebar.RadiusValues = new ArrayList() { 40.0 };
            singleRebar.Insert();
            #endregion


            //恢复工作平面
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(transformationPlane);
            model.CommitChanges();




        }







    }
}
