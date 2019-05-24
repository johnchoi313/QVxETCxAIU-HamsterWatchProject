using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AwesomeCharts {
    public class BreakChartController : MonoBehaviour {

        public LineChart chart;
        public Text nameText;
        public int nameTextDistance = 30;
        public List<DataSetFull> dataSetFulls;
        
        public void AddBreakData(string name, int day, int hour, int min) {
            //Try to find existing dataset to add to
            bool found = false;
            if(dataSetFulls.Count > 0) {
                foreach(DataSetFull dsf in dataSetFulls) {
                    if(name.ToLower().Equals(dsf.name.ToLower())) {
                        dsf.lineDataSet.AddEntry(new LineEntry(day+(hour+min/60f)/24f, hour + min/60f));
                        chart.SetDirty();
                        found = true;
                        return;
                    }
                }
            } 
            //Did not find dataset match, so create a new one:
            if(!found) {
                //Create new dataset
                Color color = Random.ColorHSV(0,1,0.5f,1,1,1,0.5f,0.5f);
                DataSetFull dataSetFull = new DataSetFull();
                dataSetFull.lineDataSet = NewLineDataSet(color);
                dataSetFull.lineDataSet.AddEntry(new LineEntry(day+(hour+min/60f)/24f, hour + min/60f));
                dataSetFull.name = name;
                dataSetFulls.Add(dataSetFull);
                chart.SetDirty();
                //Create new Player Label
                Text newText = Instantiate(nameText, nameText.transform.position, nameText.transform.rotation);
                newText.transform.parent = nameText.transform.parent;
                newText.transform.Translate(0,dataSetFulls.Count * -nameTextDistance,0);
                newText.color = new Color(color.r,color.g,color.b,0.75f);
                newText.text = name;
            }
        }

        public LineDataSet NewLineDataSet(Color color) {
            AwesomeCharts.LineDataSet dataset = new AwesomeCharts.LineDataSet();            
            dataset.FillColor = new Color32(0,0,0,0);
            dataset.LineColor = color;
            dataset.LineThickness = 0;
            chart.GetChartData().DataSets.Add(dataset);
            return dataset;
        }
    }
}

[System.Serializable]
public class DataSetFull {
    public AwesomeCharts.LineDataSet lineDataSet;
    public string name = "";
}
