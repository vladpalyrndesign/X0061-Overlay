// (C) Copyright 2018 by  
//
using System;
using System.Collections;
using System.Collections.Generic;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;


using VladGeomTools;
using VladMathTools;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(rndsgnAECACA.SetCommands))]

namespace rndsgnAECACA
{

    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is implicitly per-document!
    public class SetCommands
    {
        // The CommandMethod attribute can be applied to any public  member 
        // function of any public class.
        // The function should take no arguments and return nothing.
        // If the method is an intance member then the enclosing class is 
        // intantiated for each document. If the member is a static member then
        // the enclosing class is NOT intantiated.
        //
        // NOTE: CommandMethod has overloads where you can provide helpid and
        // context menu.

        // Modal Command with localized name
        [CommandMethod("qtest", CommandFlags.Modal)]
        public void test() // This method can have any name
        {
            /*
            "AEC_WALL"
            "AEC_WINDOW"
            "AEC_DOOR"
            */
            ObjectIdCollection ids = VladUtils.qacSSet.qF_GetObjIdByEntStrAndLayerStr("AEC_*", "", false);
            // Put your command code here
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            //Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            //Transaction tr = tm.StartTransaction();
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;


            double wallHeightToSet = 120;
            double windowsLowerHeightToSet = 10;
            double windowsUpperHeightToSet = 40;
            double precision = 0.0000;
            ///hhh
            double doorHeightToSet = 60;
            double windowHeightToSet = windowsUpperHeightToSet - windowsLowerHeightToSet;


            //ddddd
            string
                //walls
                keyWallsWrongHeights = "AEC_WALLS_WRONG_HEIGHTS",
                keyWallsOutBB = "AEC_WALLS_OUT_BB",
                //windows
                keyWindowsWrongHeights = "AEC_WINDOWS_WRONG_HEIGHTS",
                keyWindowsOutBB = "AEC_WINDOWS_OUT_BB",
                keyWindowsHigherWalls = "AEC_WINDOWS_HIGHER_WALLS",
                keyWindowsNotBottom = "AEC_WINDOWS_NOT_BOTTOM",
                keyWindowsNotTop = "AEC_WINDOWS_NOT_TOP",
                //door
                keyDoorsWrongHeights = "AEC_DOOR_WRONG_HEIGHTS",
                keyDoorsOutBB = "AEC_DOORS_WRONG_HEIGHTS",
                keyDoorsHigherWalls = "AEC_DOORS_HIGHER_WALLS",
                //rest of AEC
                keyRestOfAecOutBB = "AEC_TEST_OF_AEC";




            Dictionary<string, List<string>> AecObjectsOutBB = new Dictionary<string, List<string>>();
            //walls
            AecObjectsOutBB.Add(keyWallsWrongHeights, new List<string>());
            AecObjectsOutBB.Add(keyWallsOutBB, new List<string>());
            //windows
            AecObjectsOutBB.Add(keyWindowsWrongHeights, new List<string>());
            AecObjectsOutBB.Add(keyWindowsOutBB, new List<string>());
            AecObjectsOutBB.Add(keyWindowsHigherWalls, new List<string>());
            AecObjectsOutBB.Add(keyWindowsNotBottom, new List<string>());
            AecObjectsOutBB.Add(keyWindowsNotTop, new List<string>());
            //doors
            AecObjectsOutBB.Add(keyDoorsWrongHeights, new List<string>());
            AecObjectsOutBB.Add(keyDoorsOutBB, new List<string>());
            AecObjectsOutBB.Add(keyDoorsHigherWalls, new List<string>());
            //rest
            AecObjectsOutBB.Add(keyRestOfAecOutBB, new List<string>());



            Boolean updateWalls = true;
            Boolean updateWindows = true;
            Boolean updateDoors = true;

            Boolean commit = false;
            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                try
                {
                    using (DocumentLock acLckDoc = doc.LockDocument())
                    {
                        if (doc != null)
                        {






                            for (int i = 0; i < ids.Count; i++)
                            {
                                /// >>> WALLS >>> WALLS >>> WALLS >>> WALLS >>> WALLS >>> WALLS >>> WALLS >>> WALLS >>> WALLS >>> WALLS >>> WALLS >>> WALLS >>> WALLS >>>
                                /// walls rules 
                                /// all together:  
                                /// higher=>to default
                                /// lower=>to default
                                /// manually
                                /// set question?????
                                if (ids[i].ObjectClass.DxfName == "AEC_WALL")
                                {

                                    Autodesk.Aec.Arch.DatabaseServices.Wall wall = tr.GetObject(ids[i], OpenMode.ForRead) as Autodesk.Aec.Arch.DatabaseServices.Wall;
                                    double wallHeight = wall.BaseHeight;
                                    if (!VladMathTools.qNMath.IsSameNumbers(wallHeight, wallHeightToSet, precision))
                                    {
                                        AecObjectsOutBB[keyWallsWrongHeights].Add(ids[i].Handle.ToString());

                                        if (updateWalls)
                                        {
                                            wall.UpgradeOpen();
                                            wall.BaseHeight = wallHeightToSet;
                                            if (!commit) commit = true;
                                        }
                                    }
                                    else if (wall.GeometricExtents != null)
                                    {

                                        if (wall.GeometricExtents.MinPoint.Z != 0 || wall.GeometricExtents.MaxPoint.Z > wallHeightToSet)
                                        {
                                            AecObjectsOutBB[keyWallsOutBB].Add(ids[i].Handle.ToString());
                                        }
                                    }

                                }


                                //windows
                                /*
                                AecObjectsOutBB.Add(keyWindowsWrongHeights, new List<string>());
                                //ecObjectsOutBB.Add(keyWindowsOutBB, new List<string>());
                                //AecObjectsOutBB.Add(keyWindowsHigherWalls, new List<string>());
                                AecObjectsOutBB.Add(keyWindowsNotBottom, new List<string>());
                                AecObjectsOutBB.Add(keyWindowsNotTop, new List<string>());
            
    */


                                /// >>> WINDOWS >>>  WINDOWS >>>  WINDOWS >>>  WINDOWS >>>  WINDOWS >>>  WINDOWS >>>  WINDOWS >>>  WINDOWS >>>  WINDOWS >>>  WINDOWS >>> 
                                /// windows rules 
                                /// all together:  
                                /// height => to default
                                /// sill => to default
                                /// manually
                                /// set question?????
                                else if (ids[i].ObjectClass.DxfName == "AEC_WINDOW")
                                {
                                    Autodesk.Aec.Arch.DatabaseServices.Window window = tr.GetObject(ids[i], OpenMode.ForRead) as Autodesk.Aec.Arch.DatabaseServices.Window;
                                    double
                                        height = window.Height,
                                        bottom = window.StartPoint.Z,
                                        top = height + bottom;

                                     
                
                

                                    if (window.GeometricExtents != null)
                                    {

                                        if (window.GeometricExtents.MinPoint.Z != 0 || window.GeometricExtents.MaxPoint.Z > wallHeightToSet)
                                        {
                                            AecObjectsOutBB[keyWindowsOutBB].Add(ids[i].Handle.ToString());
                                        }
                                    }
                                    if (window.Height >= wallHeightToSet)
                                    {
                                        AecObjectsOutBB[keyWindowsHigherWalls].Add(ids[i].Handle.ToString());

                                        if (updateWindows)
                                        {
                                            window.UpgradeOpen();
                                        //    window.Height = windowHeightToSet;
                                            if (!commit) commit = true;
                                        }
                                    }


                                    /*
                                    double wallHeightToSet = 120;
                                    double windowsLowerHeightToSet = 30;ddd
                                    double windowsUpperHeightToSet = 60;
                                    double precision = 0.0000;

                                    double doorHeightToSet = 60;
                                    double windowHeightToSet = windowsUpperHeightToSet - windowsLowerHeightToSet;
                                    */



                                    if (!qNMath.IsSameNumbers(height, wallHeightToSet, precision))
                                    {
                                        AecObjectsOutBB[keyWindowsWrongHeights].Add(ids[i].Handle.ToString());

                                        if (updateWindows)
                                        {
                                            window.UpgradeOpen();
                                          //  window.Height = windowHeightToSet;
                                            if (!commit) commit = true;
                                        }
                                    }
                                     

                                    if (!qNMath.IsSameNumbers(bottom, windowsLowerHeightToSet,precision)         )                               
                                    {
                                        AecObjectsOutBB[keyWindowsNotBottom].Add(ids[i].Handle.ToString());

                                        if (updateWindows)
                                        {
                                            window.UpgradeOpen();
                                         //   window.Height = windowHeightToSet;
                                            if (!commit) commit = true;
                                        }
                                    }



                                    if (!qNMath.IsSameNumbers(top, windowsUpperHeightToSet, precision))
                                    {
                                        AecObjectsOutBB[keyWindowsNotTop].Add(ids[i].Handle.ToString());

                                        if (updateWindows)
                                        {
                                            window.UpgradeOpen();
                                          //  window.Height = windowHeightToSet;
                                            if (!commit) commit = true;
                                        }
                                    }
                                    window.UpgradeOpen();
                                  
                                    if (!commit) commit = true;


                               //     window.Height = windowHeightToSet;
                                    //Curve3d curve = window.GetGeCurve();
                                    
                                    Autodesk.AutoCAD.DatabaseServices.Extents3d bbb = window.GeometricExtents;

                                    window.Height = 50;
                                    Point3d Location = new Point3d(window.Location.X, window.Location.Y, 20);
                                    window.Location = Location;
                                    /*
                                    BoundBlock3d bb = new BoundBlock3d();
                                    Point3d MinPoint = new Point3d(bbb.MinPoint.X, bbb.MinPoint.Y, 20);
                                    Point3d MaxPoint = new Point3d(bbb.MaxPoint.X, bbb.MaxPoint.Y, 20 + 50);
                         


                                    bb.Set(MinPoint, MaxPoint);


                                    window.SetLocalExtents(bb, 3);
                                    */

                                    //  window.SetLocalExtents
                                    int jjj = 1;

                                    //    window.SetFromGeCurve(new  = new Point3d(window.StartPoint.X, window.StartPoint.Y,20);
                                    //window.EndPoint = new Point3d(window.EndPoint.X, window.EndPoint.Y, 20);
                                    


                                    //else if (Height >= WallHeightToSet)
                                    //{
                                    //    AecObjectsOutBB[keyWindowsHigherWalls].Add(ids[i].Handle.ToString());

                                    //    if (updateWindows)
                                    //    {
                                    //        window.UpgradeOpen();
                                    //        window.Height = windowHeight;
                                    //        if (!commit) commit = true;
                                    //    }
                                    //}
                                }

                                /// >>> DOORS >>>  DOORS >>>  DOORS >>>  DOORS >>>  DOORS >>>  DOORS >>>  DOORS >>>  DOORS >>>  DOORS >>>  DOORS >>> 
                                else if (ids[i].ObjectClass.DxfName == "AEC_DOOR")
                                {
                                    Autodesk.Aec.Arch.DatabaseServices.Door door = tr.GetObject(ids[i], OpenMode.ForRead) as Autodesk.Aec.Arch.DatabaseServices.Door;
                                    double Height = door.Height;
                                    if (Height >= wallHeightToSet)
                                    {
                                        AecObjectsOutBB["AEC_DOOR_WRONG_HEIGHTS"].Add(ids[i].Handle.ToString());

                                        if (updateDoors)
                                        {
                                            door.UpgradeOpen();
                                            door.Height = doorHeightToSet;
                                            if (!commit) commit = true;
                                        }
                                    }
                                    else if (door.GeometricExtents != null)
                                    {

                                        if (door.GeometricExtents.MinPoint.Z != 0 || door.GeometricExtents.MaxPoint.Z > wallHeightToSet)
                                        {
                                            AecObjectsOutBB["AEC_DOOR_OUT_BB"].Add(ids[i].Handle.ToString());
                                        }



                                    }

                                }

                                ////// >>> REST OF AEC >>>  REST OF AEC >>>  REST OF AEC >>>  REST OF AEC >>>  REST OF AEC >>>  REST OF AEC >>>  REST
                                //else if (ids[i].ObjectClass.DxfName.StartsWith ("AEC_"))
                                if (ids[i].ObjectClass.DxfName.StartsWith("AEC_"))
                                {
                                    Entity aecEntiy = tr.GetObject(ids[i], OpenMode.ForRead) as Entity;

                                    if (aecEntiy.GeometricExtents != null)
                                    {
                                        if (aecEntiy.GeometricExtents.MinPoint.Z < 0 || aecEntiy.GeometricExtents.MinPoint.Z > wallHeightToSet)
                                        {
                                            AecObjectsOutBB[keyRestOfAecOutBB].Add(ids[i].Handle.ToString());
                                        }
                                    }
                                }
                            }


                            if (commit)
                            {
                                tr.Commit();
                            }

                            ed = doc.Editor;
                            ed.WriteMessage(ids.Count.ToString());

                        }
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception e)
                {
                    tr.Abort();
                    ed.WriteMessage("\n" + e.Message + "\n");
                }
            }
        }

        //// Modal Command with pickfirst selection
        //[CommandMethod("MyGroup", "MyPickFirst", "MyPickFirstLocal", CommandFlags.Modal | CommandFlags.UsePickSet)]
        //public void MyPickFirst() // This method can have any name
        //{
        //    PromptSelectionResult result = Application.DocumentManager.MdiActiveDocument.Editor.GetSelection();
        //    if (result.Status == PromptStatus.OK)
        //    {
        //        // There are selected entities
        //        // Put your command using pickfirst set code here
        //    }
        //    else
        //    {
        //        // There are no selected entities
        //        // Put your command code here
        //    }
        //}

        //// Application Session Command with localized name
        //[CommandMethod("MyGroup", "MySessionCmd", "MySessionCmdLocal", CommandFlags.Modal | CommandFlags.Session)]
        //public void MySessionCmd() // This method can have any name
        //{
        //    // Put your command code here
        //}

        //// LispFunction is similar to CommandMethod but it creates a lisp 
        //// callable function. Many return types are supported not just string
        //// or integer.
        //[LispFunction("MyLispFunction", "MyLispFunctionLocal")]
        //public int MyLispFunction(ResultBuffer args) // This method can have any name
        //{
        //    // Put your command code here

        //    // Return a value to the AutoCAD Lisp Interpreter
        //    return 1;
        //}

    }

}
