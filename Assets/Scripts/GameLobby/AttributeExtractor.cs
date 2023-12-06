using PLATEAU.CityInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System;
using TMPro;

namespace LobbyRelaySample.ngo
{
    public class AttributeExtractor : MonoBehaviour
    {
        [SerializeField] GameObject m_AttributesPanel;
        [SerializeField] TMP_Text m_AttributesText;
        [SerializeField] Material m_MaterialSelected;
        Material m_CurrentMaterial;
        MeshRenderer m_CurrentSelectedObject;

        List<Material> m_CurrentMaterials = new List<Material>();

        public void ShowAttributes(string selectedObjectName)
        {
            if (m_CurrentSelectedObject != null)
            {
                if (m_CurrentSelectedObject.materials.Length > 1)
                {
                    m_CurrentSelectedObject.materials = m_CurrentMaterials.ToArray();
                }
                else
                {
                    m_CurrentSelectedObject.material = m_CurrentMaterial;

                }
                m_CurrentSelectedObject = null;
            }
            m_CurrentMaterials.Clear();

            // Find the selected object by name
            GameObject selectedBuilding = GameObject.Find(selectedObjectName);

            if (selectedBuilding != null)
            {
                // Selected object will be highlighted with a special material
                m_CurrentSelectedObject = selectedBuilding.GetComponent<MeshRenderer>();

                if (m_CurrentSelectedObject.materials.Length > 1)
                {
                    List<Material> selected = new List<Material>();
                    for (int i = 0; i < m_CurrentSelectedObject.materials.Length; i++)
                    {
                        selected.Add(m_MaterialSelected);
                        m_CurrentMaterials.Add(m_CurrentSelectedObject.materials[i]);
                    }
                    m_CurrentSelectedObject.materials = selected.ToArray();
                }
                else
                {
                    m_CurrentMaterial = m_CurrentSelectedObject.material;
                    m_CurrentSelectedObject.material = m_MaterialSelected;
                }
                SetAttributesOnCanvas(selectedBuilding);
            }
        }

        void SetAttributesOnCanvas(GameObject selected)
        {

            PLATEAUCityObjectGroup cityObjectGroup = selected.GetComponent<PLATEAUCityObjectGroup>();

            if (cityObjectGroup != null)
            {
                var firstPrimaryObj = cityObjectGroup.PrimaryCityObjects.FirstOrDefault();
                if (firstPrimaryObj != null && firstPrimaryObj.CityObjectType == PLATEAU.CityGML.CityObjectType.COT_Building)
                {
                    var attributesMap = firstPrimaryObj.AttributesMap;
                    if (attributesMap.TryGetValue("狩野川水系狩野川洪水浸水想定区域（想定最大規模）", out var attribute))
                    {
                        if (attribute.AttributesMapValue.TryGetValue("浸水深", out var floodLevel))
                        {
                            m_AttributesPanel.SetActive(true);
                            m_AttributesText.fontSize = 36;
                            m_AttributesText.text = "地物の浸水深：" + floodLevel.StringValue + " m";
                        }
                    }
                    else
                    {
                        m_AttributesPanel.SetActive(true);
                        m_AttributesText.text = "選択された地物は浸水深情報がありません";
                        m_AttributesText.fontSize = 22;
                    }
                }
            }
        }
    }
}