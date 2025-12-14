using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneSceneLoader : MonoBehaviour
{
    [Header("Timeline")]
    public PlayableDirector director;   // PlayableDirector (Timeline)

    [Header("Scene")]
    public string nextSceneName;         // Nama scene tujuan

    void Start()
    {
        // Jika PlayableDirector belum di-assign, ambil otomatis
        if (director == null)
        {
            director = GetComponent<PlayableDirector>();
        }

        // Daftarkan event ketika Timeline selesai
        director.stopped += OnTimelineFinished;
    }

    // Dipanggil otomatis saat Timeline selesai
    void OnTimelineFinished(PlayableDirector pd)
    {
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        // Lepas event untuk mencegah memory leak
        if (director != null)
        {
            director.stopped -= OnTimelineFinished;
        }
    }
}
