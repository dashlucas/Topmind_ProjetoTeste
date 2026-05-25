
Para testar oas features basta primeiramente seguir o caminho do botão Tools -> Mesh Combiner -> Combine Selected Objects.
    O highlight será uma feature do MeshCombine, para ver melhor cada mesh que foi combinada, bastando selecionar cada grupode combinação, possuindo um script na pasta Editor e outro na pasta Script.

    Paths:
        ShaderGraph: Assets/ShadersMaterials/Clipping/SHD_HighlightClipping.shadergraph
        Material: Assets/Materials/Mat_HighlightOverlay.mat
        ScriptParaHighlight: Assets/Scripts/HighlightController.cs
        Script do MeshCombiner: Assets/Editor/MeshCombiner.cs




Decisões técnicas:
    Mesh Combiner:
    
    - Para o mesh combiner, me utilizei do p´roprio shader de highlight para uma melhor visualização de quais meshes ficaram no agrupamento de cada material;
    - Decidi por agrupar por material automaticamente ao invés da pessoa escolher quais meshes seriam de qual material para deixar mais rápido o processo;
    - Na nomenclatura decidi por deixar o nome de 1 ou 2 objetos para facilitar a procura das meshes pelo nome dos objetos mas também com o nome do material em questão para procurar por material na cena;


    HighLight:

    - usei o Fresnel pela facilidade de se colocar já na cena, inicialmente pensei em pegar a mesh e fazer um decimante delas no blender e inverter a normal, para fazer um outline, mas ficaria ruim, somente um outline dependendo do objeto pode não ser uma boa ideia;
    - Outra forma de fazer seria embutir um sistema mais rebuscado dentro do próprio material, para fazer uma máscara utilizando ainda sim a normal no estilo do fresnel mas sem ser tão smooth ir do 0 para o 1. Um multiply no RGB poderia resolver também, pois modificaria a cor do material como um todo.
    - Obviamente não escolhi usar algum tipo de post process por conta de mobile, o Quest por  exemplo, não se deve e nem dá para usar post process por conta de performance, seria muito pesado fazer um post process nos dois olhos.


Respostas:

1: Como adaptaria este shader para melhorar performance em WebGL, Meta Quest e dispositivos Mobile?
    Não usaria transparencia dessa forma, colocaria uma máscara mais simples dentro do material base, para poder fazer um recorte usando o proprio Fresnel como delimitação, mas nada de material transparente. Uma outra forma seria usar uma versão mais simples da Mesh, inverter a normal e escalonar um pouco ela, para fazer um outline.

2: Em quais situações não é recomendado juntar meshes?
    Quando as meshes forem animadas e precisa de uma rotação individual de cada objeto, quando se tem algum tipo de animação delas, pois poderia perder a animação delas, casos de escalonamento, pois tendo que escalonar individualmente alguma meshe combinada, vai acabar escalonando todas as meshes, então basicamente qualquer tipo de modificação de transform não seria interessante fazer a combinação.

3: Quais estratégias podem ser utilizadas para reduzir shader variants dentro da Unity?
    Diminuir número de APIs Gráficas, retirar features desnecessárias para o projeto, quanto mais limpo o projeto, mais performático.