// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fracktory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;
    using HelixToolkit.Wpf;

    //[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class MainViewModel : Observable
    {
        private const string OpenFileFilter = "3D model files (*.3ds;*.obj;*.lwo;*.stl)|*.3ds;*.obj;*.objz;*.lwo;*.stl";
        private const string TitleFormatString = "3D model viewer - {0}";
        private readonly IFileDialogService fileDialogService;
        private readonly IHelixViewport3D viewport;
        private readonly Dispatcher dispatcher;
        private string currentModelPath;
        private string currentGcodePath;
        private string applicationTitle;
        private string progressString;
        private int progressValue;
        private double expansion;
        private PrintConfiguration printConfig;
        private Model3D currentModel;
        private double scaleFactor;
        private double originalScaleFactor;
        private RotateManipulator rotatorManipulatorX;
        private RotateManipulator rotatorManipulatorY;
        private RotateManipulator rotatorManipulatorZ;
        private TranslateManipulator ScaleXYZ;
        private double sizeX;
        private double sizeY;
        private double sizeZ;
        private Visibility modelLoaded;
        private Visibility modelNotLoaded;
        private Visibility modelPrint;
        private Visibility modelPrintDone;
        public SlicerAdapter adp;
        private Model3D currentMesh;
        public Model3D CurrentMesh
        {
            get
            {
                return currentMesh;
            }
            set
            {
                currentMesh = value;
                RaisePropertyChanged("CurrentMesh");
            }
        }

        private double rotateX;
        public double RotateX
        {
            get
            {
                return rotateX;
            }
            set
            {
                if (value <= 90 && value >= -90)
                {
                    rotateX = ((int)(value/15)*15);
                }

                if (CurrentModel != null)
                {
                    CurrentModel.Transform = new System.Windows.Media.Media3D.RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), rotateX));
                    CurrentModel.Transform = new System.Windows.Media.Media3D.ScaleTransform3D(scaleFactor / 100, scaleFactor / 100, scaleFactor / 100);
                }
                RaisePropertyChanged("RotateX");
            }
        }
        private double rotateY;
        public double RotateY
        {
            get
            {
                return rotateY;
            }
            set
            {
                if (value <= 90 && value >= -90)
                {
                    rotateY = ((int) (value / 15) * 15);
                }
                if (CurrentModel != null)
                {
                    CurrentModel.Transform = new System.Windows.Media.Media3D.RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), rotateY));
                    CurrentModel.Transform = new System.Windows.Media.Media3D.ScaleTransform3D(scaleFactor / 100, scaleFactor / 100, scaleFactor / 100);
                }
                RaisePropertyChanged("RotateY");

            }
        }
        private double rotateZ;
        public double RotateZ
        {
            get
            {
                return rotateZ;
            }
            set
            {
                if (value <= 90 && value >= -90)
                {
                    rotateZ = ((int) (value / 15) * 15);
                }
                if (CurrentModel != null)
                {
                    CurrentModel.Transform = new System.Windows.Media.Media3D.RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), rotateZ));
                    CurrentModel.Transform = new System.Windows.Media.Media3D.ScaleTransform3D(scaleFactor / 100, scaleFactor / 100, scaleFactor / 100);
                }
                RaisePropertyChanged("RotateZ");
            }
        }
        public Visibility ModelNotLoaded
        {
            get
            {
                return modelNotLoaded;
            }
            set
            {
                modelNotLoaded = value;
                RaisePropertyChanged("ModelNotLoaded");
            }
        }
        public Visibility ModelLoaded
        {
            get
            {
                return modelLoaded;
            }
            set
            {
                modelLoaded = value;
                RaisePropertyChanged("ModelLoaded");
            }
        }
        public Visibility ModelPrint
        {
            get
            {
                return modelPrint;
            }
            set
            {
                modelPrint = value;
                RaisePropertyChanged("ModelPrint");
            }
        }
        public Visibility ModelPrintDone
        {
            get
            {
                return modelPrintDone;
            }
            set
            {
                modelPrintDone = value;
                RaisePropertyChanged("ModelPrintDone");
            }
        }

        public double ScaleFactor
        {
            get
            {
                return this.scaleFactor;
            }
            set
            {


                if (CurrentModel != null)
                {

                    CurrentModel.Transform = new System.Windows.Media.Media3D.ScaleTransform3D(value / 100, value / 100, value / 100);
                    if (CurrentModel.Bounds.SizeX > 200 || CurrentModel.Bounds.SizeY > 200 || CurrentModel.Bounds.SizeZ > 200 || value < 0)
                    {
                        CurrentModel.Transform = new System.Windows.Media.Media3D.ScaleTransform3D(scaleFactor / 100, scaleFactor / 100, scaleFactor / 100);

                        ScaleXYZ.Position = new Point3D(0, 0, 0);
                    }
                    else
                    {
                        this.scaleFactor = value;
                        this.RaisePropertyChanged("ScaleFactor");
                        SizeX = CurrentModel.Bounds.SizeX;
                        SizeY = CurrentModel.Bounds.SizeY;
                        SizeZ = CurrentModel.Bounds.SizeZ;


                        //RaisePropertyChanged("SizeX");
                        ScaleXYZ.Length = currentModel.Bounds.SizeZ + 30;
                        if (ScaleXYZ.Visibility == Visibility.Visible)
                        {
                            ScaleXYZ.Position = new Point3D(0, 0, 0);

                        }


                    }
                }
            }
        }
        public double OriginalScaleFactor
        {
            get
            {
                return this.originalScaleFactor;
            }
            set
            {
                if (CurrentModel != null)
                {
                    originalScaleFactor = value;
                }
            }
        }
        public double SizeX
        {
            get
            {
                return sizeX;
            }
            set
            {
                sizeX = value;
                RaisePropertyChanged("SizeX");
            }
        }
        public double SizeY
        {
            get
            {
                return sizeY;
            }
            set
            {
                sizeY = value;
                RaisePropertyChanged("SizeY");
            }
        }
        public double SizeZ
        {
            get
            {
                return sizeZ;
            }
            set
            {
                sizeZ = value;
                RaisePropertyChanged("SizeZ");
            }
        }

        public string CurrentModelPath
        {
            get
            {
                return this.currentModelPath;
            }

            set
            {
                this.currentModelPath = value;
                this.RaisePropertyChanged("CurrentModelPath");
            }
        }
        public string ApplicationTitle
        {
            get
            {
                return this.applicationTitle;
            }

            set
            {
                this.applicationTitle = value;
                this.RaisePropertyChanged("ApplicationTitle");
            }
        }
        public string ProgressString
        {
            get
            {
                return progressString;
            }
            set
            {
                progressString = value;
                RaisePropertyChanged("ProgressString");
            }
        }
        public int ProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                progressValue = value;
                RaisePropertyChanged("ProgressValue");
            }
        }
        public string CurrentGcodePath
        {
            get
            {
                return this.currentGcodePath;
            }

            set
            {
                this.currentGcodePath = value;
                this.RaisePropertyChanged("CurrentGcodePath");
            }
        }

        public List<VisualViewModel> Elements
        {
            get;
            set;
        }
        public double Expansion
        {
            get
            {
                return this.expansion;
            }

            set
            {
                if (!this.expansion.Equals(value))
                {
                    this.expansion = value;
                    this.RaisePropertyChanged("Expansion");
                }
            }
        }
        public PrintConfiguration PrintConfig
        {
            get
            {
                return printConfig;
            }
            set
            {
                printConfig = value;
                this.RaisePropertyChanged("PrintConfig");
            }
        }

        public Model3D CurrentModel
        {
            get
            {
                return this.currentModel;
            }

            set
            {
                this.currentModel = value;
                SizeX = currentModel.Bounds.SizeX;
                SizeY = currentModel.Bounds.SizeY;
                SizeZ = currentModel.Bounds.SizeZ;

                this.RaisePropertyChanged("CurrentModel");

            }
        }
        public ICommand FileOpenCommand
        {
            get;
            set;
        }
        public ICommand FileExportCommand
        {
            get;
            set;
        }
        public ICommand FileExitCommand
        {
            get;
            set;
        }
        public ICommand HelpAboutCommand
        {
            get;
            set;
        }
        public ICommand ViewZoomExtentsCommand
        {
            get;
            set;
        }
        public ICommand EditCopyXamlCommand
        {
            get;
            set;
        }
        public ICommand ResetSolidCommand
        {
            get;
            set;
        }
        public ICommand ViewRotateCommand
        {
            get;
            set;
        }
        public ICommand ViewScaleCommand
        {
            get;
            set;
        }
        public ICommand PrintCommand
        {
            get;
            set;
        }
        public ICommand PronterfaceCommand
        {
            get;
            set;
        }
        public ICommand AbortCommand
        {
            get;
            set;
        }


        public MainViewModel(IFileDialogService fds, HelixViewport3D viewport, RotateManipulator rmX, RotateManipulator rmY, RotateManipulator rmZ, TranslateManipulator scaleXYZ, PrintConfiguration PrintConfig)
        {
            if (viewport == null)
            {
                throw new ArgumentNullException("viewport");
            }
            
            this.printConfig = PrintConfig;
            ScaleFactor = 100;
            SizeX = SizeY = SizeZ = 0;
            this.rotatorManipulatorX = rmX;
            this.rotatorManipulatorY = rmY;
            this.rotatorManipulatorZ = rmZ;
            this.ScaleXYZ = scaleXYZ;
            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.Expansion = 1;
            this.fileDialogService = fds;
            this.viewport = viewport;
            this.FileOpenCommand = new DelegateCommand(this.FileOpen);
            this.FileExportCommand = new DelegateCommand(this.FileExport);
            this.FileExitCommand = new DelegateCommand(FileExit);
            this.ViewZoomExtentsCommand = new DelegateCommand(this.ViewZoomExtents);
            this.EditCopyXamlCommand = new DelegateCommand(this.CopyXaml);
            this.ViewRotateCommand = new DelegateCommand(this.ViewRotate);
            this.ViewScaleCommand = new DelegateCommand(this.ViewScale);
            this.ResetSolidCommand = new DelegateCommand(this.ResetSolid);
            this.PrintCommand = new DelegateCommand(this.GenerateGCODE);
            this.PronterfaceCommand = new DelegateCommand(this.Prontercae);
            this.AbortCommand = new DelegateCommand(this.Abort);
            this.ApplicationTitle = "Fracktory";
            ModelLoaded = Visibility.Collapsed;
            ModelNotLoaded = Visibility.Visible;
            ModelPrint = Visibility.Collapsed;
            ModelPrintDone = Visibility.Collapsed;
            this.Elements = new List<VisualViewModel>();

            foreach (var c in viewport.Children)
            {
                this.Elements.Add(new VisualViewModel(c));
            }
        }

        private static void FileExit()
        {
            Application.Current.Shutdown();
        }
        private void ResetSolid()
        {
            if (CurrentModel != null)
            {
                ScaleFactor = OriginalScaleFactor;
                RotateX = RotateY = RotateZ = 0;

            }
        }
        private void FileExport()
        {
            var path = this.fileDialogService.SaveFileDialog(null, null, Exporters.Filter, ".png");
            if (path == null)
            {
                return;
            }

            this.viewport.Export(path);
        }
        private void CopyXaml()
        {
            var rd = XamlExporter.WrapInResourceDictionary(this.CurrentModel);
            Clipboard.SetText(XamlHelper.GetXaml(rd));
        }
        private void ViewZoomExtents()
        {
            this.viewport.Camera.Position = new Point3D
            {
                X = 0,
                Y = -(currentModel.Bounds.SizeY / 2) - currentModel.Bounds.SizeY,
                Z = (currentModel.Bounds.SizeZ) + currentModel.Bounds.SizeZ
            };
            this.viewport.Camera.LookDirection = new Vector3D
            {
                X = 0,
                Y = (currentModel.Bounds.SizeY / 2) + currentModel.Bounds.SizeY,
                Z = -(currentModel.Bounds.SizeZ) - currentModel.Bounds.SizeZ
            };
            viewport.Camera.UpDirection = new Vector3D(0, 0, 1);
            viewport.Camera.NearPlaneDistance = 0.001;
            viewport.CameraController.CameraRotationMode = CameraRotationMode.Turntable;
            viewport.CameraController.CameraMode = CameraMode.Inspect;
        }
        private void ViewRotate()
        {

            if (rotatorManipulatorX.Visibility == Visibility.Visible || currentModel == null)
            {
                rotatorManipulatorX.Visibility = Visibility.Collapsed;
                rotatorManipulatorY.Visibility = Visibility.Collapsed;
                rotatorManipulatorZ.Visibility = Visibility.Collapsed;


            }
            else
            {
                ScaleXYZ.Visibility = Visibility.Collapsed;
                rotatorManipulatorX.Visibility = Visibility.Visible;
                rotatorManipulatorY.Visibility = Visibility.Visible;
                rotatorManipulatorZ.Visibility = Visibility.Visible;

                double diameter = (currentModel.Bounds.SizeX > currentModel.Bounds.SizeY) ? currentModel.Bounds.SizeX : currentModel.Bounds.SizeY;
                diameter = (diameter > currentModel.Bounds.SizeZ) ? diameter : currentModel.Bounds.SizeZ;
                diameter += 50;

                rotatorManipulatorX.Diameter = diameter;
                rotatorManipulatorY.Diameter = diameter;
                rotatorManipulatorZ.Diameter = diameter;

                //inner diameter
                diameter -= 10;
                rotatorManipulatorX.InnerDiameter = diameter;
                rotatorManipulatorY.InnerDiameter = diameter;
                rotatorManipulatorZ.InnerDiameter = diameter;

            }
        }
        private void ViewScale()
        {
            if (ScaleXYZ.Visibility == Visibility.Collapsed && currentModel != null)
            {
                rotatorManipulatorX.Visibility = Visibility.Collapsed;
                rotatorManipulatorY.Visibility = Visibility.Collapsed;
                rotatorManipulatorZ.Visibility = Visibility.Collapsed;
                ScaleXYZ.Visibility = Visibility.Visible;


                ScaleXYZ.Length = currentModel.Bounds.SizeZ + 20;
                ScaleXYZ.Value = ScaleFactor;



            }
            else
            {
                ScaleXYZ.Visibility = Visibility.Collapsed;
            }
        }
        private void GenerateGCODE()
        {
            rotatorManipulatorX.Visibility = Visibility.Collapsed;
            rotatorManipulatorY.Visibility = Visibility.Collapsed;
            rotatorManipulatorZ.Visibility = Visibility.Collapsed;
            ScaleXYZ.Visibility = Visibility.Collapsed;

            adp.HasFinished = false;
            ModelPrint = Visibility.Visible;
            ProgressString = adp.ProgressOutput = "";
            ProgressValue = adp.ProgressValue = 0;
            AllowUIToUpdate();

            adp.Config = PrintConfig;
            PrintConfig.ExtraConfiguration["scale"] = (ScaleFactor / 100).ToString();
            if (RotateX == 0 && RotateY == 0 && RotateZ == 0)
            {
            }
            else
            {
                AdmeshAdapter.Rotate(CurrentModelPath, RotateX, RotateY, RotateZ);
                CurrentModelPath = CurrentModelPath.Substring(0, CurrentModelPath.IndexOf('.')) + "_rotated.stl";
            }



            ThreadPool.QueueUserWorkItem((state) =>
               {
                   Thread.Sleep(200);
                   while (adp.HasFinished == false)
                   {
                       ProgressString = adp.ProgressOutput;
                       ProgressValue = adp.ProgressValue;
                       AllowUIToUpdate();
                   }
                   ProgressString = adp.ProgressOutput;
                   ProgressValue = adp.ProgressValue;
                   AllowUIToUpdate();
               });
            ThreadPool.QueueUserWorkItem((state) =>
             {
                 adp.GenerateGcode();
                 ModelPrintDone = Visibility.Visible;
             });

        }
        public String GcodeString;
        private void Prontercae()
        {
            CurrentGcodePath = currentModelPath.Substring(0, CurrentModelPath.IndexOf('.')) + ".gcode";
            GcodeString = System.IO.File.ReadAllText(CurrentGcodePath);
            var LineList = GcodeString.Split('\n');

            double currentX = 0;
            double currentY = 0;
            double currentZ = 0;
            MeshGeometry3D mesh = new MeshGeometry3D();
            MeshBuilder builder = new MeshBuilder(false,false);
            List<Point3D> vrt = new List<Point3D>();
            foreach (var item in LineList)
            {
                Point3D? point = GParser.getPoint(item.ToString());
                if (point != null)
                {
                    Point3D Point_XYZ = (Point3D) point;
                    if (Point_XYZ.X == -1)
                    {
                        Point_XYZ.X = currentX;
                    }
                    else
                    {
                        currentX = Point_XYZ.X;
                    }
                    if (Point_XYZ.Y == -1)
                    {
                        Point_XYZ.Y = currentY;
                    }
                    else
                    {
                        currentY = Point_XYZ.Y;
                    }

                    if (Point_XYZ.Z == -1)
                    {
                        Point_XYZ.Z = currentZ;
                    }
                    else
                    {
                        currentZ = Point_XYZ.Z;
                    }

                    vrt.Add(Point_XYZ);

//                    builder.AddNode(Point_XYZ, Point_XYZ.ToVector3D(),new Point(Point_XYZ.X,Point_XYZ.Y));
 //                  builder.a
                    
                }
            }
            
            //int pos = 0;
            //for (int i = 0; i < vrt.Count; i += 5, pos++)
            //{
            //    vrt[pos] = vrt[i];
            //}
            //vrt.RemoveRange(pos, vrt.Count - pos);

            for (int i = 0; i < vrt.Count-1; i++)
            {
                builder.AddPipe(vrt[i], vrt[i + 1],0.25, 0.25, 18);
                
            }
            
            //builder.AddPolygon(vrt);
            
           // builder.AddRectangularMesh(vrt);
           //builder.AddBox(new Point3D(10, 10, 10), 20, 20, 20);
            CurrentMesh = new GeometryModel3D(builder.ToMesh(),Materials.Hue);
            RaisePropertyChanged("CurrentMesh");
            

        }
        private void Abort()
        {
            ModelPrint = Visibility.Collapsed;
            ModelPrintDone = Visibility.Collapsed;
            if (RotateX != 0 || RotateY != 0 || RotateZ != 0)
            {
                CurrentModelPath = CurrentModelPath.Substring(0, CurrentModelPath.IndexOf("_rotated.stl")) + ".stl";
            }

        }
        void AllowUIToUpdate()
        {

            DispatcherFrame frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate(object parameter)
            {

                frame.Continue = false;

                return null;

            }), null);

            Dispatcher.PushFrame(frame);

        }
        private async void FileOpen()
        {
            string path = this.fileDialogService.OpenFileDialog("models", null, OpenFileFilter, ".3ds");
            if (path == "" || path == null)
                return;

            rotatorManipulatorX.Value =
            rotatorManipulatorY.Value =
            rotatorManipulatorZ.Value = 0;

            rotatorManipulatorX.Visibility = Visibility.Collapsed;
            rotatorManipulatorY.Visibility = Visibility.Collapsed;
            rotatorManipulatorZ.Visibility = Visibility.Collapsed;
            ScaleXYZ.Visibility = Visibility.Collapsed;

            this.CurrentModelPath = path;

            adp = new SlicerAdapter(currentModelPath);

            this.CurrentModel = await this.LoadAsync(this.CurrentModelPath, false);
            this.ApplicationTitle = string.Format(TitleFormatString, this.CurrentModelPath);

            currentModel.Transform = new System.Windows.Media.Media3D.TranslateTransform3D(0, 0, -currentModel.Bounds.Z);
            ScaleFactor = adp.ScaleFactor * 100;
            OriginalScaleFactor = ScaleFactor;
            ModelLoaded = Visibility.Visible;
            ModelNotLoaded = Visibility.Collapsed;

        }
        private async Task<Model3DGroup> LoadAsync(string model3DPath, bool freeze)
        {
            return await Task.Factory.StartNew(() =>
            {
                var mi = new ModelImporter();

                if (freeze)
                {
                    // Alt 1. - freeze the model 
                    return mi.Load(model3DPath, null, true);
                }

                // Alt. 2 - create the model on the UI dispatcher
                return mi.Load(model3DPath, this.dispatcher);
            });
        }
    }
}