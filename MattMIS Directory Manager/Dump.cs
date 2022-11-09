using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattMIS_Directory_Manager
{
    class Dump
    {

		// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
		public partial class DirectoryManager
		{

			private DirectoryManagerConnection connectionField;

			private DirectoryManagerAppearance appearanceField;

			private DirectoryManagerPinnedViews pinnedViewsField;

			/// <remarks/>
			public DirectoryManagerConnection Connection
			{
				get
				{
					return this.connectionField;
				}
				set
				{
					this.connectionField = value;
				}
			}

			/// <remarks/>
			public DirectoryManagerAppearance Appearance
			{
				get
				{
					return this.appearanceField;
				}
				set
				{
					this.appearanceField = value;
				}
			}

			/// <remarks/>
			public DirectoryManagerPinnedViews PinnedViews
			{
				get
				{
					return this.pinnedViewsField;
				}
				set
				{
					this.pinnedViewsField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class DirectoryManagerConnection
		{

			private string aDUserRootField;

			private string aDTreeRootField;

			private string serverAddressField;

			private string usernameField;

			private string passwordField;

			private bool skipConnectionWizardField;

			/// <remarks/>
			public string ADUserRoot
			{
				get
				{
					return this.aDUserRootField;
				}
				set
				{
					this.aDUserRootField = value;
				}
			}

			/// <remarks/>
			public string ADTreeRoot
			{
				get
				{
					return this.aDTreeRootField;
				}
				set
				{
					this.aDTreeRootField = value;
				}
			}

			/// <remarks/>
			public string ServerAddress
			{
				get
				{
					return this.serverAddressField;
				}
				set
				{
					this.serverAddressField = value;
				}
			}

			/// <remarks/>
			public string Username
			{
				get
				{
					return this.usernameField;
				}
				set
				{
					this.usernameField = value;
				}
			}

			/// <remarks/>
			public string Password
			{
				get
				{
					return this.passwordField;
				}
				set
				{
					this.passwordField = value;
				}
			}

			/// <remarks/>
			public bool SkipConnectionWizard
			{
				get
				{
					return this.skipConnectionWizardField;
				}
				set
				{
					this.skipConnectionWizardField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class DirectoryManagerAppearance
		{

			private bool darkModeField;

			/// <remarks/>
			public bool DarkMode
			{
				get
				{
					return this.darkModeField;
				}
				set
				{
					this.darkModeField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class DirectoryManagerPinnedViews
		{

			private DirectoryManagerPinnedViewsTreeItem treeItemField;

			/// <remarks/>
			public DirectoryManagerPinnedViewsTreeItem TreeItem
			{
				get
				{
					return this.treeItemField;
				}
				set
				{
					this.treeItemField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class DirectoryManagerPinnedViewsTreeItem
		{

			private string nameField;

			private string commandField;

			private string argumentField;

			private string imageKeyField;

			private DirectoryManagerPinnedViewsTreeItemTreeItem[] childrenField;

			/// <remarks/>
			public string Name
			{
				get
				{
					return this.nameField;
				}
				set
				{
					this.nameField = value;
				}
			}

			/// <remarks/>
			public string Command
			{
				get
				{
					return this.commandField;
				}
				set
				{
					this.commandField = value;
				}
			}

			/// <remarks/>
			public string Argument
			{
				get
				{
					return this.argumentField;
				}
				set
				{
					this.argumentField = value;
				}
			}

			/// <remarks/>
			public string ImageKey
			{
				get
				{
					return this.imageKeyField;
				}
				set
				{
					this.imageKeyField = value;
				}
			}

			/// <remarks/>
			[System.Xml.Serialization.XmlArrayItemAttribute("TreeItem", IsNullable = false)]
			public DirectoryManagerPinnedViewsTreeItemTreeItem[] Children
			{
				get
				{
					return this.childrenField;
				}
				set
				{
					this.childrenField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class DirectoryManagerPinnedViewsTreeItemTreeItem
		{

			private string nameField;

			private string commandField;

			private string argumentField;

			private string imageKeyField;

			private DirectoryManagerPinnedViewsTreeItemTreeItemChildren childrenField;

			/// <remarks/>
			public string Name
			{
				get
				{
					return this.nameField;
				}
				set
				{
					this.nameField = value;
				}
			}

			/// <remarks/>
			public string Command
			{
				get
				{
					return this.commandField;
				}
				set
				{
					this.commandField = value;
				}
			}

			/// <remarks/>
			public string Argument
			{
				get
				{
					return this.argumentField;
				}
				set
				{
					this.argumentField = value;
				}
			}

			/// <remarks/>
			public string ImageKey
			{
				get
				{
					return this.imageKeyField;
				}
				set
				{
					this.imageKeyField = value;
				}
			}

			/// <remarks/>
			public DirectoryManagerPinnedViewsTreeItemTreeItemChildren Children
			{
				get
				{
					return this.childrenField;
				}
				set
				{
					this.childrenField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class DirectoryManagerPinnedViewsTreeItemTreeItemChildren
		{

			private DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItem treeItemField;

			/// <remarks/>
			public DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItem TreeItem
			{
				get
				{
					return this.treeItemField;
				}
				set
				{
					this.treeItemField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItem
		{

			private string nameField;

			private string commandField;

			private string argumentField;

			private string imageKeyField;

			private DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItemChildren childrenField;

			/// <remarks/>
			public string Name
			{
				get
				{
					return this.nameField;
				}
				set
				{
					this.nameField = value;
				}
			}

			/// <remarks/>
			public string Command
			{
				get
				{
					return this.commandField;
				}
				set
				{
					this.commandField = value;
				}
			}

			/// <remarks/>
			public string Argument
			{
				get
				{
					return this.argumentField;
				}
				set
				{
					this.argumentField = value;
				}
			}

			/// <remarks/>
			public string ImageKey
			{
				get
				{
					return this.imageKeyField;
				}
				set
				{
					this.imageKeyField = value;
				}
			}

			/// <remarks/>
			public DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItemChildren Children
			{
				get
				{
					return this.childrenField;
				}
				set
				{
					this.childrenField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItemChildren
		{

			private DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItemChildrenTreeItem treeItemField;

			/// <remarks/>
			public DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItemChildrenTreeItem TreeItem
			{
				get
				{
					return this.treeItemField;
				}
				set
				{
					this.treeItemField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItemChildrenTreeItem
		{

			private string nameField;

			private string commandField;

			private string argumentField;

			private string imageKeyField;

			private DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItemChildrenTreeItemChildren childrenField;

			/// <remarks/>
			public string Name
			{
				get
				{
					return this.nameField;
				}
				set
				{
					this.nameField = value;
				}
			}

			/// <remarks/>
			public string Command
			{
				get
				{
					return this.commandField;
				}
				set
				{
					this.commandField = value;
				}
			}

			/// <remarks/>
			public string Argument
			{
				get
				{
					return this.argumentField;
				}
				set
				{
					this.argumentField = value;
				}
			}

			/// <remarks/>
			public string ImageKey
			{
				get
				{
					return this.imageKeyField;
				}
				set
				{
					this.imageKeyField = value;
				}
			}

			/// <remarks/>
			public DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItemChildrenTreeItemChildren Children
			{
				get
				{
					return this.childrenField;
				}
				set
				{
					this.childrenField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItemChildrenTreeItemChildren
		{

			private DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItemChildrenTreeItemChildrenTreeItem treeItemField;

			/// <remarks/>
			public DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItemChildrenTreeItemChildrenTreeItem TreeItem
			{
				get
				{
					return this.treeItemField;
				}
				set
				{
					this.treeItemField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class DirectoryManagerPinnedViewsTreeItemTreeItemChildrenTreeItemChildrenTreeItemChildrenTreeItem
		{

			private string nameField;

			private string commandField;

			private string argumentField;

			private string imageKeyField;

			private object childrenField;

			/// <remarks/>
			public string Name
			{
				get
				{
					return this.nameField;
				}
				set
				{
					this.nameField = value;
				}
			}

			/// <remarks/>
			public string Command
			{
				get
				{
					return this.commandField;
				}
				set
				{
					this.commandField = value;
				}
			}

			/// <remarks/>
			public string Argument
			{
				get
				{
					return this.argumentField;
				}
				set
				{
					this.argumentField = value;
				}
			}

			/// <remarks/>
			public string ImageKey
			{
				get
				{
					return this.imageKeyField;
				}
				set
				{
					this.imageKeyField = value;
				}
			}

			/// <remarks/>
			public object Children
			{
				get
				{
					return this.childrenField;
				}
				set
				{
					this.childrenField = value;
				}
			}
		}


	}
}
