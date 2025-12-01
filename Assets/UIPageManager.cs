using UnityEngine;

public class UIPageManager : MonoBehaviour
{
    [Header("Páginas de UI (Panels dentro do Canvas)")]
    public GameObject pageMenu;        // Page_Menu
    public GameObject pageInfo;        // Page_Info
    public GameObject pageComoJogar;   // Page_ComoJogar
    public GameObject pagePuzzleHUD;   // opcional, HUD do puzzle

    [Header("Raiz do puzzle 3D (filho do ImageTarget)")]
    public GameObject puzzleRoot;      // PuzzleRoot com as peças 3D

    void Start()
    {
        ShowMenu(); // começa no menu
    }

    // Esconde todas as páginas
    public void HideAllPages()
    {
        if (pageMenu) pageMenu.SetActive(false);
        if (pageInfo) pageInfo.SetActive(false);
        if (pageComoJogar) pageComoJogar.SetActive(false);
        if (pagePuzzleHUD) pagePuzzleHUD.SetActive(false);
    }

    public void ShowMenu()
    {
        HideAllPages();
        if (pageMenu) pageMenu.SetActive(true);

        if (puzzleRoot) puzzleRoot.SetActive(false);
    }

    public void ShowInfo()
    {
        HideAllPages();
        if (pageInfo) pageInfo.SetActive(true);

        if (puzzleRoot) puzzleRoot.SetActive(false);
    }

    public void ShowComoJogar()
    {
        HideAllPages();
        if (pageComoJogar) pageComoJogar.SetActive(true);

        if (puzzleRoot) puzzleRoot.SetActive(false);
    }

    // Chamado quando o usuário realmente vai jogar
    public void ShowPuzzle()
    {
        HideAllPages();

        if (pagePuzzleHUD) pagePuzzleHUD.SetActive(true);
        if (puzzleRoot) puzzleRoot.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void TesteClique()
    {
        Debug.Log("CLICOU NO BOTÃO!");
    }
}
