using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;

namespace MattMIS_Directory_Manager
{
    public static class Config
    {

       public class Model
        {

			private RootModel rootField;

			public RootModel Default
			{
				get
				{
					return this.rootField;
				}
				set
				{
					this.rootField = value;
				}
			}


			[XmlRoot(ElementName = "Appearance")]
			public class AppearanceModel
			{
				[XmlElement(ElementName = "DarkMode")]
				public Boolean DarkMode { get; set; }
			}

			[XmlRoot(ElementName = "Connection")]
			public class ConnectionModel
			{
				[XmlElement(ElementName = "ADTreeRoot")]
				public string ADTreeRoot { get; set; }
				[XmlElement(ElementName = "ADUserRoot")]
				public string ADUserRoot { get; set; }
				[XmlElement(ElementName = "Password")]
				public string Password { get; set; }
				[XmlElement(ElementName = "ServerAddress")]
				public string ServerAddress { get; set; }
				[XmlElement(ElementName = "SkipConnectionWizard")]
				public bool SkipConnectionWizard { get; set; }
				[XmlElement(ElementName = "Username")]
				public string Username { get; set; }
			}

			[XmlRoot(ElementName = "DirectoryManager")]
			public class RootModel
			{
				[XmlElement(ElementName = "Appearance")]
				public AppearanceModel Appearance { get; set; }
				[XmlElement(ElementName = "Connection")]
				public ConnectionModel Connection { get; set; }
				[XmlElement(ElementName = "PinnedViews")]
				public PinnedViews PinnedViews { get; set; }
			}

			[XmlRoot(ElementName = "PinnedViews")]
			public class PinnedViews
			{
				[XmlElement(ElementName = "TreeItem")]
				public TreeItem TreeItem { get; set; }
			}

			[XmlRoot(ElementName = "TreeItem")]
			public class TreeItem
			{
				[XmlElement(ElementName = "Argument")]
				public string Argument { get; set; }
				
				
				[XmlArray("Children")]
				[XmlArrayItem("TreeItem")]				
				public List<TreeItem> Children { get; set; }
				
				[XmlElement(ElementName = "Command")]
				public string Command { get; set; }
				
				[XmlElement(ElementName = "ImageKey")]
				public string ImageKey { get; set; }
				
				[XmlElement(ElementName = "Name")]
				public string Name { get; set; }
			}
			
		}

        public static Model.RootModel Settings = new Model.RootModel();

        public static int LoadConfig(string path)
        {
            int failed = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                XmlSerializer serializer2 = new XmlSerializer(typeof(Model.RootModel));
                using (StringReader reader = new StringReader(doc.InnerXml))
                {
                    Settings = (Model.RootModel)serializer2.Deserialize(reader);

                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load configuration file. \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                failed = 1;
            }
            return failed;
        }





        

    }
}
