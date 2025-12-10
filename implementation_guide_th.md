# IMPLEMENTATION\_GUIDE\_TH

คู่มือนี้จะอธิบายขั้นตอนอย่างละเอียดในการตั้งค่าโปรเจกต์ Unity เพื่อให้ใช้งาน **Miranova Engine (VSNL)** ได้

> **ข้อควรรู้**: ตรวจสอบให้แน่ใจว่าไฟล์ C# script ทั้งหมดอยู่ในโฟลเดอร์ `Assets/Miralis/Scripts` ก่อนเริ่มทำตามขั้นตอน

***

## 1. การตั้งค่าโปรเจกต์และเลเยอร์ (Project Settings & Layers)

ก่อนสร้าง Scene เราต้องกำหนดลำดับการแสดงผล (Sorting Layer) สำหรับภาพ 2D เสียก่อน

* [ ] **เปิดไปที่:** `Edit` -> `Project Settings` -> `Tags and Layers`
* [ ] **กำหนด Sorting Layers** (เรียงลำดับจากบนลงล่าง):
  1. `Default` (พื้นฐาน)
  2. `Background` (รูปฉากหลัง)
  3. `Video` (พื้นผิวสำหรับวิดีโอ)
  4. `Character` (ตัวละคร Sprites / Live2D)
  5. `VFX` (เอฟเฟกต์ต่างๆ)
  6. `UI` (กล่องข้อความ, ปุ่มตัวเลือก)
* [ ] **กำหนด Tags**:
  * กดปุ่ม `+` เพื่อเพิ่ม Tag ใหม่ชื่อ: `SpawnRoot` (ใช้สำหรับจุดเกิดของวัตถุแบบ Dynamic)

***

## 2. โครงสร้างของฉาก (Bootstrap Scene)

สร้าง Scene ใหม่ชื่อ `Boot.unity` และตั้งค่า Hierarchy ให้เหมือนด้านล่างนี้เป๊ะๆ:

### 2.1 ตัวจัดการหลัก (The Engine Root)

* [ ] สร้าง **Empty GameObject** ตั้งชื่อว่า `[MiranovaEngine]`
* [ ] **เพิ่ม Component**: ลากไฟล์ `Engine.cs` ใส่เข้าไป
* [ ] **สำคัญ**: เพื่อให้ปรับแต่งผ่าน Inspector ได้ง่าย ให้เพิ่ม Service Component อื่นๆ เข้าไปด้วย Engine จะได้เจอทันทีโดยไม่ต้องสร้าง Empty Object เปล่าๆ
  * กด Add Component: `UIManager`
  * กด Add Component: `CharacterManager`
  * กด Add Component: `AudioManager`
  * กด Add Component: `BackgroundManager`

### 2.2 โครงสร้างหน้าจอ UI

* [ ] สร้าง **UI -> Canvas** ตั้งชื่อว่า `MainCanvas`
  * Render Mode: `Screen Space - Overlay`
  * Canvas Scaler: `Scale With Screen Size` (ตั้งค่า 1920 x 1080)
  * Sorting Layer: เลือก `UI`
* [ ] สร้างลูก (Child): **Panel** ตั้งชื่อว่า `BackgroundPanel` (เอา Image source ออก และปรับสี Alpha เป็น 0)
  * _ตัวนี้ใช้เป็นที่อยู่ของรูปภาพฉากหลัง ถ้าคุณไม่ได้ใช้ Sprite Renderer_
* [ ] สร้างลูก (Child): **Panel** ตั้งชื่อว่า `DialoguePanel` (ตั้ง Anchor เป็นด้านล่างเต็มจอ / Bottom Stretch)
  * ใส่ `Image` (สีดำ, Alpha 0.8) เพื่อทำเป็นพื้นหลังกล่องข้อความ
  * สร้างลูก: `Text (TMP)` ตั้งชื่อว่า **SpeakerName** (ชื่อตัวละคร)
  * สร้างลูก: `Text (TMP)` ตั้งชื่อว่า **DialogueLine** (บทพูด)
* [ ] สร้างลูก (Child): **Panel** ตั้งชื่อว่า `ChoicePanel` (ตั้ง Anchor เป็นตรงกลาง / Center Middle)
  * ใส่ `VerticalLayoutGroup` (Spacing 10, Alignment Center)
  * สร้างลูก: **Button** ตั้งชื่อว่า `ChoiceButton_Prefab` (ให้ลากปุ่มนี้ไปเก็บใน `Assets/Prefabs/UI` เพื่อทำเป็น Prefab แล้วลบออกจาก Scene)

***

## 3. การเชื่อมต่อ Prefab และ Inspector

ตอนนี้เราจะมาเชื่อมของในฉากเข้ากับโค้ดใน `[MiranovaEngine]`

### 3.1 ตั้งค่า UIManager

คลิกที่ `[MiranovaEngine]` แล้วหา **UIManager** Component

* [ ] **Dialogue Panel**: ลาก `MainCanvas/DialoguePanel` มาใส่
* [ ] **Speaker Text**: ลาก `MainCanvas/DialoguePanel/SpeakerName` มาใส่
* [ ] **Dialogue Text**: ลาก `MainCanvas/DialoguePanel/DialogueLine` มาใส่
* [ ] **Choice Panel**: ลาก `MainCanvas/ChoicePanel` มาใส่
* [ ] **Choice Container**: ลาก `MainCanvas/ChoicePanel` (ตัวเดิม) มาใส่
* [ ] **Choice Button Prefab**: ลากไฟล์ Prefab `ChoiceButton` ที่เราทำไว้เมื่อกี้จากโฟลเดอร์ Project มาใส่

### 3.2 ตั้งค่า CharacterManager

คลิกที่ `[MiranovaEngine]` แล้วหา **CharacterManager** Component

* [ ] **Character Root**: ให้สร้าง Empty GameObject ใหม่ไว้ใต้ `MainCanvas` ตั้งชื่อว่า `CharacterRoot` แล้วลากมาใส่ตรงนี้ (เพื่อให้ตัวละครแสดงผลทับฉากหลัง แต่อยู่ใต้ UI)
* [ ] **Metadata DB**: ถ้ามีไฟล์ `CharacterMetadata.asset` ให้ลากมาใส่ (คลิกขวา `Create -> VSNL -> CharacterMetadata`) หรือปล่อยว่างไว้ก็ได้ถ้าจะโหลดรูปตรงๆ

### 3.3 ตั้งค่าเสียง (Audio)

* [ ] **Music Folder**: ตรวจสอบให้แน่ใจว่ามีโฟลเดอร์ `Assets/Miralis/Resources/Audio/BGM` ตัว Engine จะโหลดเพลงจากที่นี่

***

## 4. การตั้งค่า Addressables (ละเอียดมาก / Super Detailed)

ระบบนี้ช่วยให้เกมโหลดไฟล์ได้เร็วและจัดการง่าย แต่ต้องตั้งค่าให้ถูกทุกขั้นตอนครับ

{% stepper %}
{% step %}
### ติดตั้ง (Installation)

* ไปที่เมนู **Window** -> **Package Manager**
* ที่มุมซ้ายบน เลือก **Packages: Unity Registry**
* ช่องค้นหาขวาบน พิมพ์ว่า `Addressables`
* กดปุ่ม **Install** (มุมขวาล่าง) และรอจนเสร็จ
{% endstep %}

{% step %}
### เริ่มต้นระบบ (Initialization)

* ไปที่เมนู **Window** -> **Asset Management** -> **Addressables** -> **Groups**
* จะมีหน้าต่างใหม่เด้งขึ้นมา ถ้าเห็นปุ่มเขียนว่า **"Create Addressables Settings"** ให้กดเลย!
* ตอนนี้คุณจะเห็น Group ชื่อว่า `Default Local Group (Default)`
{% endstep %}

{% step %}
### การจัดกลุ่ม (Groups)

* คลิกขวาในพื้นที่ว่างของหน้าต่าง Addressables Groups -> **Create New Group** -> **Packed Assets**
* ตั้งชื่อว่า `Backgrounds`
* ทำซ้ำเพื่อสร้างกลุ่ม `Audio` และ `Characters`
{% endstep %}

{% step %}
### การลงทะเบียนไฟล์ (Mark Assets)

สมมติคุณมีรูปฉากหลังชื่อ `school_day.png` อยู่ในโฟลเดอร์ Project:

* ลากไฟล์ `school_day.png` จาก Project Window เข้ามาใส่ในกลุ่ม `Backgrounds` ที่เราเพิ่งสร้างในหน้าต่าง Addressables
* **สำคัญที่สุด**: ตอนนี้ชื่อไฟล์ใน Addressables จะยาวมาก (เช่น `Assets/Miralis/Resources/Backgrounds/school_day.png`)
* **ให้คลิกขวาที่ชื่อยาวๆ นั้น** -> **Rename**
* แก้ให้เหลือแค่ `school_day` (นี่คือ **"Address"** หรือ **"กุญแจ"** ที่เราจะใช้ในโค้ด)
* _ถ้าในสคริปต์เราเขียน `@bg "school_day"` ตัวเกมจะมาหาไฟล์ที่มี Address ชื่อนี้_
{% endstep %}

{% step %}
### การ Build (ห้ามลืม!)

ถ้าไม่ทำข้อนี เกมจะหาไฟล์ไม่เจอ:

* ในหน้าต่าง Addressables Groups
* คลิกเมนู **Build** (แถบด้านบนของหน้าต่าง) -> **New Build** -> **Default Build Script**
* รอสักครู่... เสร็จแล้ว!

Tips: ทุกครั้งที่คุณเพิ่มไฟล์ใหม่ หรือเปลี่ยนชื่อ Address คุณต้องกด **Build -> Update a Previous Build** หรือ New Build เสมอเพื่ออัปเดตระบบ
{% endstep %}
{% endstepper %}

***

## 5. การปรับโค้ดครั้งสุดท้าย (จำเป็น)

เนื่องจากในขั้นตอนที่ 2.1 เราทำการ Add Component เองเพื่อให้ตั้งค่าได้ เราต้องแน่ใจว่าโค้ด `Engine.cs` จะไม่ไปสร้างซ้ำ

**เปิดไฟล์ `Assets/Miralis/Scripts/Core/Engine.cs` หาฟังก์ชัน `InitializeEngineAsync` และเช็คว่ามีการใช้ `GetComponent` ก่อน `AddComponent` ตามตัวอย่างนี้หรือไม่:**

```csharp
var uiManager = gameObject.GetComponent<UIManager>();
if (!uiManager) uiManager = gameObject.AddComponent<UIManager>();
```

_(โค้ดนี้ถูกแก้ไขให้แล้วโดยอัตโนมัติ แต่นำมาแสดงไว้เพื่อความเข้าใจ)_

***

## 6. การเชื่อมต่อ Logic และการตั้งค่า Engine

Engine จะรู้ได้อย่างไรว่าจะต้องเล่นไฟล์ไหน?

**วิธีตั้งค่าในหน้า Inspector ของ `Engine`:**

### 6.1 ไฟล์เริ่มต้น (Start Script)

* **Start Script**: ลากไฟล์ `.vsnl` (เช่น `Start.vsnl`) จากช่อง Project มาใส่ในช่องนี้
  * _ถ้าปล่อยว่างไว้ Engine จะพยายามหาไฟล์ชื่อ "Start" ในโฟลเดอร์ Resources โดยอัตโนมัติ_
* **Auto Start**: ติ๊กถูกไว้ (`True`) เพื่อให้เริ่มเล่นทันทีที่กด Play

### 6.2 การตั้งค่า Service Overrides (Optional / ตัวเลือกเสริม)

คุณจะเห็นรายการ Manager มากมายเช่น `Save Load Manager`, `UI Manager` ฯลฯ

* **แบบอัตโนมัติ (แนะนำ)**: ถ้า Component ทั้งหมดอยู่บน **GameObject เดียวกัน** กับ Engine คุณสามารถ **ปล่อยให้เป็น `None` ได้เลย** Engine แสนฉลาดตัวนี้จะหาเจอเอง
* **แบบกำหนดเอง**: คุณสามารถลาก Component จาก Inspector มาใส่ในช่องเหล่านี้ได้ ถ้าคุณอยากมั่นใจ 100% ว่ามันเชื่อมกันถูกตัว

***

## 7. ปัญหาที่พบบ่อย (Troubleshooting) & ช่องว่างใน Inspector

ถ้าคุณเห็นช่องว่างๆ ใน Inspector (เหมือนในภาพที่คุณเคยส่งมา) ให้ทำดังนี้:

### 7.1 Text Manager เป็นช่องว่าง (`None`)?

ถ้าช่อง **Text Manager** ใน `UIManager` ขึ้นว่า `None`:

1. **Add Component**: คลิกที่ `[MiranovaEngine]` แล้วกดปุ่ม Add Component พิมพ์หา `TextDisplayManager` แล้วเพิ่มเข้าไป
2. **Assign (เชื่อมต่อ)**: ลากตัว `[MiranovaEngine]` (ตัวมันเองนี่แหละ) เข้าไปใส่ในช่อง **Text Manager** ของ `UIManager`
3. **ตั้งค่า TextDisplayManager**:
   * **Speaker Text**: ลาก `MainCanvas/DialoguePanel/SpeakerName` มาใส่
   * **Dialogue Text**: ลาก `MainCanvas/DialoguePanel/DialogueLine` มาใส่
   * _หมายเหตุ: ตัวนี้แยกออกมาเพื่อรับผิดชอบเอฟเฟกต์ "ตัวหนังสือค่อยๆ พิมพ์" โดยเฉพาะ_

***

## 8. ไฟล์เสียงหรือรูปหาย (FAQ)

<details>

<summary>คำถาม: "ยังไม่มีไฟล์เพลงหรือเสียงประกอบ เกมจะพังไหม?"</summary>

**คำตอบ: ไม่พังครับ**

Engine ถูกออกแบบมาให้รับมือกับไฟล์ที่หายไปได้

* ถ้าคุณเขียนคำสั่ง `@bgm "EpicTheme"` แต่ไม่มีไฟล์นั้นอยู่ Console จะขึ้นเตือนสีเหลืองว่า **Warning**: `[AudioManager] Clip EpicTheme not found`
* ตัวเกมจะ **เล่นต่อไปได้ตามปกติ** (แค่ไม่มีเสียง)

**วิธีแนะนำสำหรับช่วงทดสอบ:**

1. **ข้ามคำสั่งไปก่อน**: อย่าเพิ่งเขียนคำสั่ง `@bgm` จนกว่าจะมีไฟล์
2. **ใช้ไฟล์หลอก (Placeholders)**:
   * สร้างไฟล์เสียงเปล่าๆ หรือเสียงอะไรก็ได้มาไฟล์นึง
   * ตั้งชื่อว่า `placeholder.mp3`
   * เขียนในสคริปต์ว่า `@bgm "placeholder"`
   * ถ้าใช้ Addressables: ให้ตั้งชื่อ Address ของไฟล์หลอกๆ นั้นว่า `BattleTheme` ไปก่อน พอได้ไฟล์จริงมา ก็แค่เอาไฟล์จริงไปทับ แล้วตั้งชื่อ Address เหมือนเดิม คุณแทบไม่ต้องแก้โค้ดเลย!

</details>

***

เสร็จสมบูรณ์! ลองกด Play ดูได้เลย Engine จะเริ่มทำงาน ตั้งค่าระบบ และมองหาไฟล์ `Start.vsnl` (หรือไฟล์ที่คุณเลือกไว้) เพื่อเริ่มเล่าเรื่อง
