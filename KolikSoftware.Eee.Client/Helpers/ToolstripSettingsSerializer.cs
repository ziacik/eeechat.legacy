using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;

namespace KolikSoftware.Eee.Client.Helpers
{
    sealed class ToolstripSettingsSerializer
    {
        public static string Serialize(ToolStripContainer toolStripContainer)
        {
            StringBuilder builder = new StringBuilder();

            using (XmlWriter writer = XmlTextWriter.Create(builder))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("ToolStrips");

                WritePanel(writer, "Top", toolStripContainer.TopToolStripPanel);
                WritePanel(writer, "Left", toolStripContainer.LeftToolStripPanel);
                WritePanel(writer, "Right", toolStripContainer.RightToolStripPanel);
                WritePanel(writer, "Bottom", toolStripContainer.BottomToolStripPanel);

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }

            return builder.ToString();
        }

        static void WritePanel(XmlWriter writer, string which, ToolStripPanel toolStripPanel)
        {
            writer.WriteStartElement(which);

            int rowIndex = 0;
            int counter = 0;

            SortedDictionary<int, ToolStrip> toolStripsSorted = new SortedDictionary<int, ToolStrip>();

            foreach (ToolStripPanelRow toolStripRow in toolStripPanel.Rows)
            {
                foreach (ToolStrip toolStrip in toolStripRow.Controls)
                {
                    /// Counter is there just to be sure that if there were some toolstrips witch the same position, this does not go down.
                    toolStripsSorted.Add(-toolStrip.Left * 100 - counter, toolStrip);
                    counter++;
                }

                foreach (ToolStrip toolStrip in toolStripsSorted.Values)
                {
                    writer.WriteStartElement("Strip");
                    writer.WriteAttributeString("row", rowIndex.ToString());
                    writer.WriteAttributeString("name", toolStrip.Name);
                    writer.WriteAttributeString("left", toolStrip.Left.ToString());
                    writer.WriteAttributeString("top", toolStrip.Top.ToString());
                    writer.WriteAttributeString("stretch", toolStrip.Stretch.ToString());
                    writer.WriteEndElement();
                }

                rowIndex++;

                toolStripsSorted.Clear();
            }

            writer.WriteEndElement();
        }

        public static void Deserialize(ToolStripContainer toolStripContainer, string settings)
        {
            if (settings != null && settings.Length > 0)
            {
                toolStripContainer.SuspendLayout();

                try
                {
                    Dictionary<string, ToolStrip> toolStrips = new Dictionary<string, ToolStrip>();

                    foreach (ToolStrip toolStrip in toolStripContainer.TopToolStripPanel.Controls)
                    {
                        toolStrips.Add(toolStrip.Name, toolStrip);
                    }

                    foreach (ToolStrip toolStrip in toolStripContainer.LeftToolStripPanel.Controls)
                    {
                        toolStrips.Add(toolStrip.Name, toolStrip);
                    }

                    foreach (ToolStrip toolStrip in toolStripContainer.RightToolStripPanel.Controls)
                    {
                        toolStrips.Add(toolStrip.Name, toolStrip);
                    }

                    foreach (ToolStrip toolStrip in toolStripContainer.BottomToolStripPanel.Controls)
                    {
                        toolStrips.Add(toolStrip.Name, toolStrip);
                    }

                    XmlDocument document = new XmlDocument();
                    document.LoadXml(settings);

                    DeserializeOne(toolStripContainer.TopToolStripPanel, "Top", document, toolStrips);
                    DeserializeOne(toolStripContainer.LeftToolStripPanel, "Left", document, toolStrips);
                    DeserializeOne(toolStripContainer.RightToolStripPanel, "Right", document, toolStrips);
                    DeserializeOne(toolStripContainer.BottomToolStripPanel, "Bottom", document, toolStrips);

                    foreach (ToolStrip remainingStrip in toolStrips.Values)
                    {
                        if (remainingStrip.Name == "actionsToolStrip")
                            toolStripContainer.TopToolStripPanel.Join(remainingStrip, 0);
                        else if (remainingStrip.Name == "UsersToolStrip")
                            toolStripContainer.TopToolStripPanel.Join(remainingStrip, 1);
                        else if (remainingStrip.Name == "RoomsToolStrip")
                            toolStripContainer.BottomToolStripPanel.Join(remainingStrip, 0);
                        else if (remainingStrip.Name == "statusStrip")
                            toolStripContainer.BottomToolStripPanel.Join(remainingStrip, 1);
                    }
                }
                finally
                {
                    toolStripContainer.ResumeLayout();
                }
            }
        }

        static void DeserializeOne(ToolStripPanel toolStripPanel, string which, XmlDocument document, Dictionary<string, ToolStrip> toolStrips)
        {
            toolStripPanel.Controls.Clear();

            foreach (XmlNode node in document.SelectNodes("/*/" + which + "/Strip"))
            {
                try
                {
                    string name = node.Attributes["name"].Value;

                    if (toolStrips.ContainsKey(name))
                    {
                        ToolStrip strip = toolStrips[name];
                        strip.Stretch = bool.Parse(node.Attributes["stretch"].Value);
                        int row = int.Parse(node.Attributes["row"].Value);
                        strip.Left = int.Parse(node.Attributes["left"].Value);
                        strip.Top = int.Parse(node.Attributes["top"].Value);
                        toolStripPanel.Join(strip, row);
                        toolStrips.Remove(name);
                    }
                }
                catch (Exception)
                {
                    //TODO: Log error
                }
            }
        }
    }
}
