using UnityEngine;
using System.Collections;

public class RunePiece : MonoBehaviour
{
    [Header("Koneksi (Saat Rotasi 0)")]
    // Tentukan di inspector: Ke mana rune ini nyambung di awal?
    public bool connectsTop;
    public bool connectsRight;
    public bool connectsBottom;
    public bool connectsLeft;

    [Header("Status")]
    public bool isPowered = false; // Apakah dialiri energi?
    private Renderer myRenderer; // Untuk mengubah warna/cahaya

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        UpdateVisuals();
    }

    // Fungsi untuk dipanggil saat pemain mengklik rune ini
    public void RotatePiece()
    {
        // 1. Putar Logika Koneksi (90 derajat searah jarum jam)
        // Atas jadi Kanan, Kanan jadi Bawah, dst.
        bool tempTop = connectsTop;
        connectsTop = connectsLeft;
        connectsLeft = connectsBottom;
        connectsBottom = connectsRight;
        connectsRight = tempTop;

        // 2. Putar Visual Objeknya
        // Kita putar sumbu Y atau Z tergantung orientasi model Anda.
        // Anggap kita memutar di sumbu Y (jika rune ditaruh mendatar di meja).
        transform.Rotate(new Vector3(0, 90, 0));

        // 3. Beri tahu Manager untuk cek ulang semua koneksi
        // (Kita akan buat managernya nanti)
        if (RunePuzzleManager.instance != null)
        {
            RunePuzzleManager.instance.CheckPuzzleConnections();
        }
    }

    // Fungsi untuk mengubah tampilan jika teraliri energi
    public void SetPowered(bool powered)
    {
        isPowered = powered;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        // Contoh simpel: Ubah warna material. 
        // Nanti Anda bisa ganti dengan menyalakan Emission pada shader.
        if (isPowered)
        {
            myRenderer.material.color = Color.cyan; // Warna menyala
        }
        else
        {
            myRenderer.material.color = Color.gray; // Warna batu mati
        }
    }

    // Deteksi Klik Mouse
    void OnMouseDown()
    {
        // Jangan putar jika ini adalah potongan Sumber atau Tujuan (opsional)
        // if (!bisaDiputar) return; 
        
        RotatePiece();
    }
}