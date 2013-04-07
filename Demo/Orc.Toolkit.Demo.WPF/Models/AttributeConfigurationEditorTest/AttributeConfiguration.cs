using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;

namespace Orc.Toolkit.Demo.Models
{
    /// <summary>
    /// Describes .net type
    /// </summary>
    public class AttributeType
    {
        /// <summary>
        /// Gets or sets the class name.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the system type.
        /// </summary>
        public string SystemType { get; set; }
    }

    public class AttributeConfiguration
    {
        /// <summary>
        /// List allowed to be selected for attribute
        /// </summary>
        public static readonly List<AttributeType> SupportedTypes = new List<AttributeType> 
                {
                    new AttributeType { ClassName = "System.String", SystemType = "System.String", DisplayName = "String" },
                    new AttributeType { ClassName = "Link", SystemType = "System.String", DisplayName = "Link" },
                    new AttributeType { ClassName = "System.DateTime", SystemType = "System.DateTime", DisplayName = "DateTime" },
                    new AttributeType { ClassName = "System.Boolean", SystemType = "System.Boolean", DisplayName = "Boolean" },
                    new AttributeType { ClassName = "System.Double", SystemType = "System.Double", DisplayName = "Double" },
                    new AttributeType { ClassName = "System.Int32", SystemType = "System.Int32", DisplayName = "Integer" }
                };
        #region Fields

        /// <summary>
        /// The values.
        /// </summary>
        private readonly HashSet<string> values = new HashSet<string>();

        /// <summary>
        /// The colors.
        /// </summary>
        private readonly Dictionary<string, string> colors = new Dictionary<string, string>();

        /// <summary>
        /// visibility dictionary - for each attribute value whether it visible or not
        /// </summary>
        private readonly Dictionary<string, bool> visibility = new Dictionary<string, bool>();

        private int detailWindowPosition;
        private int editorPosition;
        private bool isDisabled;
        private bool isInDetailsWindow;
        private bool isInEditor;
        private bool isInTooltip;
        private bool isInLabel;
        private string name;
        private int tooltipPosition;
        private int labelPosition;
        private string type;
        private string category;
        private string description;
        private bool isColorAttribute;

        #endregion

        #region Constructors and Destructors
        
        public AttributeConfiguration()
        {
        }

        #endregion

        #region Public Properties        

        public int DetailWindowPosition
        {
            get
            {
                return this.detailWindowPosition;
            }

            set
            {
                if (this.detailWindowPosition != value)
                {
                    this.detailWindowPosition = value;                    
                }
            }
        }

        public int EditorPosition
        {
            get
            {
                return this.editorPosition;
            }

            set
            {
                if (this.editorPosition != value)
                {
                    this.editorPosition = value;
                }
            }
        }

        public bool IsDisabled
        {
            get
            {
                return this.isDisabled;
            }

            set
            {
                if (this.isDisabled != value)
                {
                    this.isDisabled = value;
                }
            }
        }

        public bool IsInDetailsWindow
        {
            get
            {
                return this.isInDetailsWindow;
            }

            set
            {
                if (this.isInDetailsWindow != value)
                {
                    this.isInDetailsWindow = value;
                }
            }
        }

        public bool IsInEditor
        {
            get
            {
                return this.isInEditor;
            }

            set
            {
                if (this.isInEditor != value)
                {
                    this.isInEditor = value;
                }
            }
        }

        public bool IsInTooltip
        {
            get
            {
                return this.isInTooltip;
            }

            set
            {
                if (this.isInTooltip != value)
                {
                    this.isInTooltip = value;
                }
            }
        }

        public bool IsInLabel
        {
            get
            {
                return this.isInLabel;
            }

            set
            {
                if (this.isInLabel != value)
                {
                    this.isInLabel = value;
                }
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        public int TooltipPosition
        {
            get
            {
                return this.tooltipPosition;
            }

            set
            {
                if (this.tooltipPosition != value)
                {
                    this.tooltipPosition = value;
                }
            }
        }

        public int LabelPosition
        {
            get
            {
                return this.labelPosition;
            }

            set
            {
                if (this.labelPosition != value)
                {
                    this.labelPosition = value;
                }
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }

            set
            {
                if (this.type != value)
                {
                    this.type = value;
                }
            }
        }

        
        public string Category
        {
            get
            {
                return this.category;
            }

            set
            {
                if (this.category != value)
                {
                    this.category = value;
                }
            }
        }
        
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                if (this.description != value)
                {
                    this.description = value;
                }
            }
        }
                
        public bool IsColorAttribute
        {
            get
            {
                return this.isColorAttribute;
            }

            set
            {
                if (this.isColorAttribute != value)
                {
                    this.isColorAttribute = value;
                }
            }
        }

        public IEnumerable<object> Values
        {
            get
            {
                return this.values;
            }
        }

        #endregion

    }

    public class AttributeConfigurations : ObservableCollection<AttributeConfiguration>
    {
        public AttributeConfigurations()
        {
            var attributeConfiguration1 = new AttributeConfiguration()
            {
                Category = "Category 1",
                Description = "AttributeConfiguration 1 description",
                Name = "Attribute configuration 1",
                Type = "System.String"
            };
            this.Add(attributeConfiguration1);

            var attributeConfiguration2 = new AttributeConfiguration()
            {
                Category = "Category 2",
                Description = "AttributeConfiguration 2 description",
                Name = "Attribute configuration 2",
                Type = "System.Int32"
            };
            this.Add(attributeConfiguration2);

            var attributeConfiguration3 = new AttributeConfiguration()
            {
                Category = "Category 3",
                Description = "AttributeConfiguration 3 description",
                Name = "Attribute configuration 3",
                Type = "System.Double"
            };
            this.Add(attributeConfiguration3);
        }
    }
}
