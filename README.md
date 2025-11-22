<div align="center">

# 🎮 [2D Co-Op Multiplayer Platformer]

![Start](https://github.com/user-attachments/assets/d8d87d5e-6ed1-44b7-88f3-440d9974643a)


![Cover](BURAYA_ANA_GIF_LINKI.gif)

**Unity** • **Mirror Networking** • **C#**

</div>

---

## 📝 Proje Hakkında
Arkadaşınızla birlikte engelleri aşıp bulmacaları çözdüğünüz 2 kişilik bir 2D platform oyunudur. Bu proje, **gerçek zamanlı veri senkronizasyonu** ve **sunucu tabanlı (server-authoritative)** oyun mimarisini öğrenmek amacıyla geliştirilmiştir.

---

## 🔧 Teknik Mimari (Mirror Implementation)
Bu projede Unity'nin Mirror kütüphanesi kullanılarak şu teknik yapılar kurulmuştur:

| Kullanılan Yapı | Nerede/Neden Kullanıldı? |
| :--- | :--- |
| **Server Authority** | Hile koruması ve senkronizasyon için tüm fizik hesaplamaları sunucuda yapılıp istemciye gönderildi. |
| **[SyncVar] & Hooks** | Oyuncu canı ve skor takibi için kullanıldı. Değişken sunucuda değiştiğinde `UpdateUI` fonksiyonu tetiklenerek arayüz güncellendi. |
| **[Command]** | İstemciden (Client) sunucuya istek atmak için. (Örn: Oyuncunun "Kapıyı Aç" tuşuna basması). |
| **[ClientRpc]** | Sunucudan tüm oyunculara görsel efekt göndermek için. (Örn: Bir oyuncu öldüğünde çıkan patlama efekti). |
| **NetworkTransform** | Oyuncuların pozisyon ve rotasyonunun akıcı bir şekilde (interpolasyon ile) diğer ekranlarda görünmesi için. |

---

## ✨ Özellikler
- 🕹️ **2 Kişilik Co-Op Oynanış:** Bulmacalar tek başına çözülemez, iş birliği gerekir.
- 🏃 **Fizik Tabanlı Hareket:** Zıplama, dash atma ve engellerden kaçınma.
- 🌐 **Lobby Sistemi:** Oyuncuların bağlanıp "Hazır" vermesini bekleyen bekleme odası.

---

<div align="center">
  <a href="BURAYA_YOUTUBE_VIDEO_LINKI">
    

https://github.com/user-attachments/assets/04436799-f539-4f90-a796-6fbbfe9f215b





  </a>
</div>


---


## 🚀 Kurulum ve Test
Bu proje yerel ağ (LAN) veya Localhost üzerinde test edilebilir.

1. **Releases** kısmından `Build.zip` dosyasını indirin.
2. `.exe` dosyasını **iki kere** çalıştırın (İki pencere açın).
3. Birinde **Host**, diğerinde **Client** butonuna basın.
4. İyi eğlenceler!

---
<div align="center">

*Geliştirici: [Senin Adın]* *İletişim: [Mail veya LinkedIn Linkin]*

</div>
