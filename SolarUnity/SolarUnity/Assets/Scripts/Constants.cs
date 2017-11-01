using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Constants : MonoBehaviour
{
    public List<Area> Areas = new List<Area>();

    public void Setup()
    {
        Areas.Add((new Area("Area 27", GenerateStationList(5))));
        Areas.Add((new Area("Area 25", GenerateStationList(5))));
        Areas.Add((new Area("Area 21", GenerateStationList(5))));
        Areas.Add((new Area("Area 28", GenerateStationList(5))));
        Areas.Add((new Area("Area 29", GenerateStationList(5))));
        Areas.Add((new Area("P & Q", GenerateStationList(5))));
    }



    List<Station> GenerateStationList(int count)
    {
        List<Station> stations = new List<Station>();
        for (int i = 0; i < count; i++)
        {
            stations.Add(new Station("Station " + i, 5, 5, GenerateOrderBoxes(5), GenerateShelfBoxes(5)));
        }
        return stations;
    }


    List<OrderBox> GenerateOrderBoxes(int count)
    {
        List<OrderBox> orderBoxes = new List<OrderBox>();
        for (int i = 0; i < count; i++)
        {
            orderBoxes.Add(new OrderBox("OB"));
        }
        return orderBoxes;
    }


    List<ShelfBox> GenerateShelfBoxes(int count)
    {
        List<ShelfBox> shelfBoxes = new List<ShelfBox>();
        for (int i = 0; i < count; i++)
        {
            shelfBoxes.Add(new ShelfBox("SB"));
        }
        return shelfBoxes;
    }



}
