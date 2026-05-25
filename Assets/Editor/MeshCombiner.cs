using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MeshCombiner
{
    [MenuItem("Tools/Mesh Combiner/Combine Selected Objects")]
    public static void CombineMeshes()
    {
        GameObject[] selectedObjects = Selection.gameObjects; //Pega os objetos selecionados na hierarquia

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Nenhum game object foi selecionado! Por favor, selecione as meshes da scene!");
            return;
        }

        List<MeshFilter> validMeshFilters = new List<MeshFilter>(); //Lista para armazenar os MeshFilters válidos

        foreach (GameObject obj in selectedObjects)
        {
            MeshFilter filter = obj.GetComponent<MeshFilter>(); //Pega o MeshFilter do objeto
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>(); //Pega o MeshRenderer do objeto

            if (filter != null && renderer != null && filter.sharedMesh != null) // Verifica se tem o component de rendere, mesh filter ou se está só vazio
            {
                validMeshFilters.Add(filter);
            }
            else
            {
                Debug.LogWarning($"O objeto '{obj.name}' não possui um MeshFilter ou MeshRenderer e será ignorado.");
            }

        }
        Debug.LogWarning($"Possui '{validMeshFilters.Count}' meshes válidas!");

        Dictionary<Material, List<MeshFilter>> materialGroups = new Dictionary<Material, List<MeshFilter>>(); //Agrupar por materiais em umn dicionario

        foreach(MeshFilter filter in validMeshFilters)
        {
            MeshRenderer renderer = filter.GetComponent<MeshRenderer> ();

            Material mat = renderer.sharedMaterial;

            if (!materialGroups.ContainsKey(mat))
            {
                materialGroups.Add(mat, new List<MeshFilter>());
            }

            materialGroups[mat].Add(filter);

        }
        Debug.Log($"{validMeshFilters.Count} malhas em {materialGroups.Count} grupos de materiais diferentes.");

        List<GameObject> newCombinedObjects = new List<GameObject>();

        foreach (KeyValuePair<Material, List<MeshFilter>> kvp in materialGroups)
        {
            // O tipo é Material
            Material mat = kvp.Key;

            // O tipo é MeshFilter, a variável é filters
            List<MeshFilter> filters = kvp.Value;

            CombineInstance[] combineInstances = new CombineInstance[filters.Count];

            //Tudo padronizado como 'filters'
            for (int i = 0; i < filters.Count; i++)
            {
                combineInstances[i].mesh = filters[i].sharedMesh;
                combineInstances[i].transform = filters[i].transform.localToWorldMatrix;
                filters[i].gameObject.SetActive(false);
            }

            string baseName = filters[0].gameObject.name; // Pega o nome do primeiro objeto da lista

            if (filters.Count > 1)
            {
                // Se houver mais de um, adiciona o contador de filhos absorvidos
                baseName += $"_e_{filters.Count - 1}_outros";
            }


            Mesh combinedMesh = new Mesh();
            combinedMesh.name = $"{baseName}_Mesh";

            combinedMesh.CombineMeshes(combineInstances, true, true);

            // Nome final do GameObject que vai aparecer na Hierarchy da Unity
            GameObject combinedObject = new GameObject($"{baseName}_({mat.name})");

            MeshFilter combinedFilter = combinedObject.AddComponent<MeshFilter>();
            MeshRenderer combinedRenderer = combinedObject.AddComponent<MeshRenderer>();

            combinedFilter.sharedMesh = combinedMesh;


            // A malha nasce apenas com o material base limpo
            combinedRenderer.sharedMaterial = mat;

            // Anexamos o novo script controlador ao objeto criado
            HighlightController controller = combinedObject.AddComponent<HighlightController>();

            // Sorteamos a cor aleatória e entregamos para o controlador
            controller.corDoHighlight = Random.ColorHSV(0f, 1f, 0.7f, 1f, 1f, 1f);

            // Adicionamos o objeto na lista para ser selecionado no final
            newCombinedObjects.Add(combinedObject);
        }

        // Seleciona as malhas novas automaticamente na aba Hierarchy
        Selection.objects = newCombinedObjects.ToArray();
        Debug.Log("Juncao feita com sucesso! Os originais foram desativados e os novos selecionados.");
    }
}
