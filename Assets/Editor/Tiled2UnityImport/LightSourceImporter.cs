using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Tiled2Unity.CustomTiledImporter]
public class LightSourceImporter : Tiled2Unity.ICustomTiledImporter {

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> props) {

        if(gameObject && gameObject.transform.parent.name == "lightSource") {
            Vector3 gameObjectCenter = gameObject.GetComponent<Collider2D>().bounds.center;
            GameObject lightObject = new GameObject(gameObject.name);
            lightObject.transform.parent = gameObject.transform.parent;
            lightObject.transform.position = gameObjectCenter;

            float zHeight = MapConstants.MEDIUM_HEIGHT;
            if(props.ContainsKey("zHeight")) {
                switch(props["zHeight"]) {
                    case "floor":
                        zHeight = MapConstants.FLOOR_HEIGHT;
                        break;
                    case "medium":
                        break;
                    case "ceiling":
                        zHeight = MapConstants.CEILING_HEIGHT;
                        break;
                }
            }
            float range = MapConstants.MEDIUM_RANGE;
            if(props.ContainsKey("range")) {
                switch(props["range"]) {
                    case "low":
                        range = MapConstants.LOW_RANGE;
                        break;
                    case "medium":
                        break;
                    case "high":
                        range = MapConstants.HIGH_RANGE;
                        break;
                }
            }
            float intensity = MapConstants.MEDIUM_INTENSITY;
            if(props.ContainsKey("intensity")) {
                switch(props["intensity"]) {
                    case "low":
                        intensity = MapConstants.LOW_INTENSITY;
                        break;
                    case "medium":
                        break;
                    case "high":
                        intensity = MapConstants.HIGH_INTENSITY;
                        break;
                }
            }

            Color lightColor = props.ContainsKey("color") ? (Color)typeof(Color).GetProperty(props["color"]).GetValue(null, null) : Color.white;
            LightType lightType = props.ContainsKey("type") ? (LightType)System.Enum.Parse(typeof(LightType), props["type"]) : LightType.Point;

            LightShadows lightShadows = ((props.ContainsKey("shadows") && props["shadows"] == "true") || zHeight == MapConstants.FLOOR_HEIGHT) ? LightShadows.Soft : LightShadows.None;

            Light light = lightObject.AddComponent<Light>();
            lightObject.transform.position = new Vector3(lightObject.transform.position.x, lightObject.transform.position.y, zHeight);
            light.range = range;
            light.intensity = intensity;
            light.type = lightType;
            light.color = lightColor;
            light.shadows = lightShadows;

            Object.DestroyImmediate(gameObject);
        }
        
    }

    public void CustomizePrefab(GameObject gameObject) {
        
    }
}
