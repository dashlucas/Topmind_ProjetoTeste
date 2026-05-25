using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class HighlightController : MonoBehaviour
{
    [Header("Configuracoes Visuais")]
    public Color corDoHighlight = Color.green;
    [Range(0.1f, 10f)]
    public float intensidade = 2f;

    [Header("Configuracoes do Clipping Plane")]
    public Vector3 posicaoDoCorte;
    public Vector3 normalDoCorte = Vector3.up;

    private Material highlightMaterialInstance;
    private MeshRenderer meshRenderer;

    private const string NOME_ASSINATURA = "Highlight_Temporario_Over";

    void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();

#if UNITY_EDITOR
        EditorApplication.update += ChecarSelecao;
#endif
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= ChecarSelecao;
#endif
        RemoverHighlight();
    }

#if UNITY_EDITOR
    void ChecarSelecao()
    {
        if (meshRenderer == null) return;

        bool selecionado = Selection.Contains(gameObject);
        bool temHighlight = TemHighlightNaMalha();

        if (selecionado && !temHighlight)
        {
            AdicionarHighlight();
        }
        else if (!selecionado && temHighlight)
        {
            RemoverHighlight();
        }
        else if (selecionado && temHighlight && highlightMaterialInstance != null)
        {
            // Atualiza o brilho apenas com ele selecionado
            highlightMaterialInstance.SetColor("_Color", corDoHighlight);
            highlightMaterialInstance.SetFloat("_Power", intensidade);
            highlightMaterialInstance.SetVector("_ClippingPosition", posicaoDoCorte);
            highlightMaterialInstance.SetVector("_ClippingNormal", normalDoCorte);
        }
    }

    bool TemHighlightNaMalha()
    {
        Material[] mats = meshRenderer.sharedMaterials;
        for (int i = 0; i < mats.Length; i++)
        {
            // Procura em TODAS as gavetas pela assinatura do highlight
            if (mats[i] != null && mats[i].name.Contains(NOME_ASSINATURA))
                return true;
        }
        return false;
    }

    private void AdicionarHighlight()
    {
        // Destroi duplicacoes antes de tentar adicionar um novo
        RemoverHighlight();

        Material baseMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_HighlightOverlay.mat");
        if (baseMat == null) return;

        highlightMaterialInstance = new Material(baseMat);
        highlightMaterialInstance.name = NOME_ASSINATURA; 
        highlightMaterialInstance.hideFlags = HideFlags.DontSave;

        // Injeta na ultima gaveta
        Material[] mats = meshRenderer.sharedMaterials;
        Material[] newMats = new Material[mats.Length + 1];
        mats.CopyTo(newMats, 0);
        newMats[newMats.Length - 1] = highlightMaterialInstance;
        
        meshRenderer.sharedMaterials = newMats;
    }

    private void RemoverHighlight()
    {
        if (meshRenderer == null) return;

        Material[] mats = meshRenderer.sharedMaterials;
        List<Material> limpos = new List<Material>();
        bool precisaAtualizar = false;

        // Varre todo o array e salva APENAS o que NAO FOR o Highlight
        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i] != null && mats[i].name.Contains(NOME_ASSINATURA))
            {
                precisaAtualizar = true;
                continue; // Mata o highlight
            }
            limpos.Add(mats[i]); // Mantem a parede de tijolos intacta
        }

        if (precisaAtualizar)
        {
            meshRenderer.sharedMaterials = limpos.ToArray();
            
            // Obriga a placa de video a apagar o brilho da tela no clique
            SceneView.RepaintAll(); 
        }
    }
#endif
}