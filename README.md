# [Oyunun İsmi] - Multiplayer Co-Op Game

Bu proje, **Unity** ve **Mirror Networking** kullanılarak geliştirilmiş, [2] kişilik bir online co-op oyun prototipidir. Projenin temel amacı, Server-Authoritative mimariyi anlamak ve gerçek zamanlı veri senkronizasyonu (State Synchronization) konusunda yetkinlik kazanmaktır.

![Gameplay Demo](Buraya_Gif_Linki_Gelecek.gif)

## 🔗 Linkler
- **Oynanabilir Demo (Build):** [Itch.io veya GitHub Releases Linki]
- **Oynanış Videosu:** [YouTube Video Linki - Kesinlikle Tavsiye Edilir]

---

## 🛠️ Teknik Özellikler & Mirror Kullanımı
Bu projede **High Level API (Mirror)** kullanılarak aşağıdaki network mekanikleri implemente edilmiştir:

### 1. Server Authority & Movement
Hile koruması ve senkronizasyon bütünlüğü için hareket mekaniği sunucu otoritelidir.
- **Client-Side Prediction:** Oyuncunun input gecikmesi hissetmemesi için yerel hareket anında işlenir, sunucu onayı arka planda gerçekleşir.
- **Transform Sync:** Pozisyon ve rotasyon verileri `NetworkTransform` bileşeni ile optimize edilmiş şekilde senkronize edilir.

### 2. State Synchronization (SyncVar & Hooks)
Oyun içi değişkenlerin tüm clientlarda aynı olması sağlanmıştır.
- **Can ve Skor Sistemi:** `[SyncVar(hook = nameof(OnHealthChanged))]` yapısı kullanılarak, sunucuda değişen can değeri anında UI'a yansıtılır.
- **Lobby Sistemi:** Oyuncuların hazır olma durumları ve lobiye giriş çıkışları senkronize edilir.

### 3. Remote Procedure Calls (RPCs)
- **[Command]:** Oyuncuların nesnelerle etkileşime girmesi (örn: kapı açma, ateş etme) istemciden sunucuya komut olarak gönderilir.
- **[ClientRpc] / [TargetRpc]:** Sunucu, oyunun başladığını veya özel efektlerin (partikül, ses) çalışmasını tetiklemek için istemcilere mesaj gönderir.

![Sync Demo](Buraya_Sync_Gif_Linki_Gelecek.gif)

---

## 🎮 Nasıl Test Edilir? (Local Multiplayer)
Bu oyunu test etmek için ikinci bir bilgisayara veya Hamachi'ye ihtiyacınız yoktur. Tek bilgisayarda şu adımları izleyebilirsiniz:

1. **Build'i İndirin:** Releases kısmından `.zip` dosyasını indirin.
2. **İki Pencere Açın:** Oyunun `.exe` dosyasını **iki kez** çalıştırın (İki ayrı pencere açılacak).
3. **Host Olun:** Birinci pencerede **"Host (Server + Client)"** butonuna tıklayın.
4. **Bağlanın:** İkinci pencerede adres kısmına `localhost` yazın (veya boş bırakın) ve **"Client"** butonuna tıklayın.
5. **Hazırsınız!** Artık iki karakteri de aynı bilgisayardan kontrol ederek senkronizasyonu test edebilirsiniz.

---

## 💻 Geliştirme Süreci & Kazanımlar
Bu proje sayesinde şunları deneyimledim:
- Unity Networking (UNet) mantığının Mirror üzerindeki modern uygulaması.
- Race condition ve latency yönetimi.
- Multiplayer oyunlarda "Spawn" ve "Object Pooling" yönetimi.

---

*Geliştirici: [Senin Adın]*
*İletişim: [Linkedin veya Email]*
