﻿using Module.Welcome.Model;
using Module.Welcome.PrismEvent;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Module.Welcome.ViewModel
{
    public class LastProjectsViewModel: BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly string BasePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private readonly string SubPath = @"\FOMODplist.xml";

        public ProjectLinkList ProjectLinkList { get; set; } = new ProjectLinkList();


        //TO DO - сделать кликабельным список последник проектов

        public LastProjectsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;


            var list = ReadProjectLinkListFile();
            if (list!=null)
            {
                ProjectLinkList = list;
                OnPropertyChanged(nameof(ProjectLinkList));
            }

            
            _eventAggregator.GetEvent<OpenProjectEvent>().Subscribe( p =>
            {
               var project = ProjectLinkList.Links.FirstOrDefault(i => i.FolderPath == p);
                if (project==null)
                {
                    ProjectLinkList.Links.Add(new ProjectLinkModel{FolderPath = p});
                    SaveProjectLinkListFile();
                }
            });
        }




        private ProjectLinkList ReadProjectLinkListFile()
        {
            if (File.Exists(BasePath + SubPath))
            {
                try
                {
                    return DeserializeObject<ProjectLinkList>(BasePath + SubPath);
                }
                catch (Exception e)
                {
                    throw;
                }
            }

            return null;
        }
        private bool SaveProjectLinkListFile()
        {
            if (Directory.Exists(BasePath))
            {
                try
                {
                    SerializeObject(ProjectLinkList, BasePath + SubPath);
                    return true;
                }
                catch (Exception e)
                {
                    throw;
                }
            }

            return false;
        }



        #region MyRegion


        private void SerializeObject<T>(T data, string path)
        {
            if (data == null) return;
            using (var fs = File.Create(path))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(fs, data);
            }
        }
        private T DeserializeObject<T>(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(fs);
            }
        }

        #endregion


      
    }
}
