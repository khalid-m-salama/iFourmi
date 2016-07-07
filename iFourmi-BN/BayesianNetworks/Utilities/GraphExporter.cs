using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace iFourmi.BayesianNetworks.Utilities
{
    public static class GraphExporter
    {
        public static string ExportToGaphSharpXml(BayesianNetworks.Model.BayesianNetworkClassifier graph)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<graphml>");
            builder.Append(Environment.NewLine);
            builder.Append("<graph id=\""+graph.Metadata.DatasetName+"\" ");
            builder.Append("edgedefault=\"directed\" ");
            builder.Append("parse.nodes=\""+graph.Variables.Length.ToString()+"\" ");
            builder.Append("parse.edges=\"" + graph.Edges.Length.ToString() + "\" ");

            builder.Append("parse.order=\"nodesfirst\" ");
            builder.Append("parse.nodeids=\"free\" ");
            builder.Append("parse.edgeids=\"free\" >");
            builder.Append(Environment.NewLine);

            foreach (BayesianNetworks.Model.Variable variable in graph.Variables)
            {     
                builder.Append("<node id=\"" + variable.Attribute.Name + "\" />");
                builder.Append(Environment.NewLine);
            }

            int counter = 0;
            foreach (BayesianNetworks.Model.Edge edge in graph.Edges)
            {
                string parent = graph.Metadata.Target.Index==edge.ParentIndex?graph.Metadata.Target.Name: graph.Metadata.Attributes[edge.ParentIndex].Name;
                string child = graph.Metadata.Target.Index == edge.ChildIndex ? graph.Metadata.Target.Name : graph.Metadata.Attributes[edge.ChildIndex].Name;

                builder.Append("<edge id=\"" + counter.ToString() + "\" source=\"" +  parent + "\" target=\"" + child + "\" />");                
                builder.Append(Environment.NewLine);
                counter++;
            }

            builder.Append("</graph>");
            builder.Append(Environment.NewLine);
            builder.Append("</graphml>");
            return builder.ToString();
         
        }
    }
}
