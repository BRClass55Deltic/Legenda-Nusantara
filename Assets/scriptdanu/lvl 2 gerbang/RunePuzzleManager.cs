using UnityEngine;
using System.Collections.Generic;

public class RunePuzzleManager : MonoBehaviour
{
    public static RunePuzzleManager instance;

    [Header("Grid Setup")]
    // Masukkan semua rune di scene ke dalam list ini secara manual atau lewat code
    public List<RunePiece> allRunes = new List<RunePiece>();
    public RunePiece startRune; // Rune sumber energi
    public RunePiece endRune;   // Rune tujuan (yang membuka gerbang)

    [Header("Target Gerbang")]
    public GameObject gateObject; // Objek gerbang yang mau dibuka

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Cek koneksi di awal game
        CheckPuzzleConnections();
    }

    // FUNGSI UTAMA: Dipanggil setiap kali ada rune yang diputar
    public void CheckPuzzleConnections()
    {
        // 1. Reset semua rune jadi mati dulu
        foreach (RunePiece rune in allRunes)
        {
            rune.SetPowered(false);
        }

        // 2. Mulai alirkan energi dari Sumber
        // Kita gunakan 'HashSet' untuk mencatat rune mana yang sudah dicek agar tidak loop tak berujung
        HashSet<RunePiece> visitedRunes = new HashSet<RunePiece>();
        PowerUpRecursive(startRune, visitedRunes);

        // 3. Cek apakah Rune Tujuan berhasil mendapatkan energi
        if (endRune.isPowered)
        {
            Debug.Log("PUZZLE SELESAI! Buka Gerbang.");
            OpenGate();
        }
    }

    // Fungsi Rekursif untuk "menjalara" ke tetangga
    void PowerUpRecursive(RunePiece currentRune, HashSet<RunePiece> visited)
    {
        // Jika rune ini sudah dicek atau null, berhenti.
        if (currentRune == null || visited.Contains(currentRune)) return;

        // Nyalakan rune ini
        currentRune.SetPowered(true);
        visited.Add(currentRune);

        // --- CEK TETANGGA ---
        // Ini bagian yang agak rumit. Kita harus tahu siapa tetangga di Atas, Kanan, Bawah, Kiri.
        // Untuk tutorial ini, kita pakai cara simpel: Raycast.
        
        float rayDistance = 1.5f; // Sesuaikan dengan jarak antar rune di grid Anda

        // CEK ATAS (Jika rune ini punya koneksi Atas)
        if (currentRune.connectsTop)
        {
            RunePiece neighbor = GetNeighbor(currentRune.transform.position, Vector3.forward, rayDistance); // Asumsi grid di lantai (Z forward)
            // "Jabat Tangan": Apakah tetangga ada, DAN dia punya koneksi BAWAH?
            if (neighbor != null && neighbor.connectsBottom)
            {
                PowerUpRecursive(neighbor, visited);
            }
        }

        // CEK KANAN
        if (currentRune.connectsRight)
        {
            RunePiece neighbor = GetNeighbor(currentRune.transform.position, Vector3.right, rayDistance);
             // "Jabat Tangan": Apakah tetangga punya koneksi KIRI?
            if (neighbor != null && neighbor.connectsLeft)
            {
                PowerUpRecursive(neighbor, visited);
            }
        }
        
        // ... Lakukan hal yang sama untuk connectsBottom (cek neighbor.connectsTop)
        // ... Lakukan hal yang sama untuk connectsLeft (cek neighbor.connectsRight)
        // (Demi keringkasan kode, silakan lengkapi 2 arah sisanya)
    }

    // Helper untuk mencari tetangga pakai Raycast
    RunePiece GetNeighbor(Vector3 startPos, Vector3 direction, float distance)
    {
        RaycastHit hit;
        // Tembak raycast dari tengah rune ke arah yang diminta
        if (Physics.Raycast(startPos, direction, out hit, distance))
        {
            RunePiece foundPiece = hit.collider.GetComponent<RunePiece>();
            if (foundPiece != null)
            {
                return foundPiece;
            }
        }
        return null;
    }

    void OpenGate()
    {
        // Logika membuka gerbang di sini.
        // Misalnya: Play animasi, atau hilangkan objek gerbang.
        if(gateObject != null)
        {
            // Contoh simpel: Nonaktifkan gerbang
            gateObject.SetActive(false);
             // Atau jika punya animator: gateAnimator.SetTrigger("Open");
        }
    }
}