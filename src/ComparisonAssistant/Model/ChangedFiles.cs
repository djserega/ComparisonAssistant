using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonAssistant.Model
{
    public class ChangedFiles
    {
        private string _fileName;

        public string Status { get; set; }
        public string FileName
        {
            get => _fileName;
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    ParseFileName();
                }
            }
        }
        public bool ChangeObject { get; private set; }
        public bool ChangeModuleObject { get; private set; }
        public bool ChangeModuleManaged { get; private set; }
        public bool ChangeTemplate { get; private set; }

        internal string FilePart0 { get; private set; }
        internal string FilePart1 { get; private set; }
        internal string FilePart2 { get; private set; }
        internal string FilePart3 { get; private set; }
        internal string FilePart4 { get; private set; }
        internal string FilePart5 { get; private set; }
        internal string FilePart6 { get; private set; }
        internal string FilePart7 { get; private set; }
        internal string FilePart8 { get; private set; }
        internal string FilePart9 { get; private set; }

        internal bool EndFileXML { get; private set; }
        internal bool EndFileBSL { get; private set; }

        internal int LastFilledPart { get; private set; }

        internal string FileNameWithoutLastPart { get; private set; }
        internal string FileNameWithoutExtension { get; private set; }

        public ChangedFiles(string status, string fileName)
        {
            Status = status;
            FileName = fileName;
        }

        private void ParseFileName()
        {
            CheckEndingFileName();
            SplitIntoParts();

            if (EndFileBSL || EndFileXML)
            {
                FileNameWithoutLastPart = FileNameWithoutLastPart.RemoveEndText("/");
                if (FilePart2 == "Ext")
                    FileNameWithoutLastPart = FileNameWithoutLastPart.RemoveEndText("/Ext");
            }

            CheckChangedType();
        }

        private void CheckEndingFileName()
        {
            EndFileBSL = FileName.EndsWith(".bsl");
            EndFileXML = FileName.EndsWith(".xml");

            if (EndFileBSL)
                FileNameWithoutExtension = FileName.RemoveEndText(".bsl");
            else if (EndFileXML)
                FileNameWithoutExtension = FileName.RemoveEndText(".xml");
        }

        private void SplitIntoParts()
        {
            StringBuilder stringBuilderLastPart = new StringBuilder();

            string[] fileNameParts = _fileName.Split('/');
            int countParts = fileNameParts.Count();
            for (int i = 0; i < 10; i++)
            {
                string part;
                if (countParts > i)
                {
                    part = fileNameParts[i];
                    if (countParts - 1 > i)
                    {
                        stringBuilderLastPart.Append(part);
                        stringBuilderLastPart.Append("/");
                    }
                    LastFilledPart = i;
                }
                else
                    part = string.Empty;

                switch (i)
                {
                    case 0:
                        FilePart0 = part;
                        break;
                    case 1:
                        FilePart1 = part;
                        break;
                    case 2:
                        FilePart2 = part;
                        break;
                    case 3:
                        FilePart3 = part;
                        break;
                    case 4:
                        FilePart4 = part;
                        break;
                    case 5:
                        FilePart5 = part;
                        break;
                    case 6:
                        FilePart6 = part;
                        break;
                    case 7:
                        FilePart7 = part;
                        break;
                    case 8:
                        FilePart8 = part;
                        break;
                    case 9:
                        FilePart9 = part;
                        break;
                }
            }

            FileNameWithoutLastPart = stringBuilderLastPart.ToString();
        }

        private void CheckChangedType()
        {
            if (FilePart2 == "Forms")
            {
                if (FilePart4 == "Ext")
                {
                    if (FilePart6 == "Module.bsl")
                        ChangeModuleObject = true;
                    else if (FilePart5.EndsWith(".xml"))
                        ChangeObject = true;
                }
                else if (FilePart3.EndsWith(".xml"))
                    ChangeObject = true;
            }
            else if (FilePart2 == "Templates")
            {
                ChangeTemplate = true;
            }
            else if (FilePart2 == "Subsystems")
            {
                if (FilePart3.EndsWith(".xml"))
                    ChangeObject = true;
                else if (FilePart4 == "Subsystems")
                {
                    if (FilePart5.EndsWith(".xml"))
                        ChangeObject = true;
                    else if (FilePart6 == "Ext")
                    {
                        if (FilePart7.EndsWith(".xml"))
                            ChangeObject = true;
                    }
                }
            }
            else if (FilePart2 == "Commands")
            {
                if (FilePart5.EndsWith(".bsl"))
                    ChangeModuleObject = true;
            }
            else if (FilePart2 == "Ext")
            {
                if (FilePart3 == "Module.bsl")
                    ChangeModuleObject = true;
                else if (FilePart3 == "ManagerModule.bsl")
                    ChangeModuleManaged = true;
                else if (FilePart3 == "ObjectModule.bsl")
                    ChangeModuleObject = true;
            }
            else if (FilePart1.EndsWith(".xml"))
                ChangeObject = true;
            else if (FilePart0.EndsWith(".xml"))
                ChangeObject = true;
        }

        internal void CompareObject(ChangedFiles changed)
        {
            if (changed.ChangeModuleManaged)
                ChangeModuleManaged = true;

            if (changed.ChangeModuleObject)
                ChangeModuleObject = true;

            if (changed.ChangeObject)
                ChangeObject = true;

            if (changed.ChangeTemplate)
                ChangeTemplate = true;
        }
    }
}
