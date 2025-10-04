# README.fr.md — Documentation du projet SignalR Bus Demo

## 🚍 Démonstration SignalR – Suivi en temps réel des autobus (.NET 8 MVC)

Cette application illustre comment utiliser **SignalR** avec **ASP.NET Core MVC** pour diffuser en temps réel l’avancement de plusieurs autobus sur différentes lignes. Le projet sert de base pour comprendre les concepts d’abonnement aux groupes, de mise à jour en direct et de tableau de bord d’administration.

---

### 1️⃣ Vue d’ensemble de l’architecture

L’application est composée de deux parties principales :

* **Backend (C#, .NET 8 MVC + SignalR)**

  * Hubs pour la communication temps réel.
  * Services pour générer et diffuser l’état des autobus.
  * Suivi des connexions utilisateurs et de leur temps de présence.

* **Frontend (Views MVC + JavaScript)**

  * Pages de lignes de bus affichant la progression d’un trajet.
  * Tableau de bord administratif (Dashboard) pour surveiller l’état global et contrôler la diffusion.

L’interaction entre les deux passe exclusivement par **SignalR**, qui maintient une connexion WebSocket bidirectionnelle.

---

### 2️⃣ Backend — Composants clés

**Program.cs**

* Configure les services MVC et SignalR.
* Enregistre les services **`BusStateStore`**, **`ConnectionTracker`** et **`BusBackgroundService`**.
* Mappe les hubs SignalR :

  * `/bushub` pour les pages de lignes.
  * `/dashboardhub` pour le tableau de bord.

**BusHub**

* Point d’entrée pour les clients (pages de lignes).
* Permet de :

  * **JoinGroup / LeaveGroup** : rejoindre ou quitter une ligne d’autobus (chaque ligne = un “groupe” SignalR).
  * Surveiller les déconnexions et mettre à jour le suivi des utilisateurs.

**DashboardHub**

* Canal pour la page Dashboard.
* Reçoit les mises à jour globales (état de chaque ligne, connexions actives).
* Permet des actions administratives (pause de diffusion, reprise, déconnexion d’un utilisateur spécifique).

**BusBackgroundService**

* Tâche de fond qui simule le déplacement des autobus :

  * Toutes les **5 secondes**, chaque ligne avance potentiellement d’un arrêt.
  * Redémarre automatiquement le trajet lorsque la fin est atteinte.
* Diffuse l’état mis à jour vers :

  * Les groupes correspondants (`BusHub`) pour les usagers.
  * Le Dashboard pour la supervision.

**BusStateStore**

* Stocke l’état actuel de chaque ligne (arrêt courant, nombre total d’arrêts, pourcentage complété).

**ConnectionTracker**

* Enregistre les connexions actives par ligne.
* Permet de savoir combien d’utilisateurs sont abonnés à chaque bus.
* Stocke aussi l’heure de connexion de chaque utilisateur pour calculer la durée en ligne.

**ConsoleColors**

* Ajoute des messages colorés dans la console du serveur (pratique pour déboguer et suivre les événements en direct).

---

### 3️⃣ Frontend — Pages MVC et SignalR côté client

**Line.cshtml (Vue unique pour toutes les lignes)**

* Page dynamique qui reçoit le `groupName` et le `title` depuis le contrôleur.
* Affiche :

  * Barre de progression correspondant à l’avancement de l’autobus.
  * Numéro d’arrêt courant / nombre total.
* Boutons intégrés :

  * **Connect / Disconnect** : gérer la connexion SignalR.
  * **Join Group / Leave Group** : rejoindre ou quitter la ligne sélectionnée.
* Écoute les événements `BusProgress` pour mettre à jour l’interface en temps réel.

**Dashboard.cshtml**

* Vue pour les administrateurs ou superviseurs.
* Affiche un tableau récapitulatif :

  * Chaque ligne avec son avancement, nombre d’usagers connectés et statut (actif ou en pause).
* Liste les connexions individuelles avec :

  * ID de connexion, heure d’arrivée et durée de présence.
  * Bouton **Disconnect** pour déconnecter un utilisateur spécifique.
* Contrôles pour **Pause Group** / **Resume Group** afin de stopper ou relancer la diffusion d’une ligne.

---

### 4️⃣ Fonctionnement en temps réel

1. **Connexion initiale**

   * Le navigateur ouvre une connexion SignalR au hub approprié (`/bushub` pour les lignes, `/dashboardhub` pour le tableau de bord).

2. **Abonnement à une ligne**

   * L’utilisateur clique sur “Join Group” ou arrive directement sur l’URL `/Home/Line/{id}`.
   * Le client rejoint un **groupe SignalR** représentant la ligne (ex. `group-a` pour “Line A”).

3. **Simulation du déplacement**

   * Le service `BusBackgroundService` incrémente régulièrement l’avancement des bus.
   * Chaque fois qu’un bus progresse, un message est envoyé à son groupe.

4. **Mises à jour Dashboard**

   * Le Dashboard reçoit l’état global (progrès + nombre d’usagers connectés).
   * L’interface affiche en direct la progression et la liste des connexions.

5. **Contrôle en direct**

   * Depuis le Dashboard, un admin peut **pauser** la diffusion d’une ligne (ex. maintenance ou bus hors service).
   * Possibilité de déconnecter un utilisateur problématique.

---

### 5️⃣ Points forts pour un projet réel

* **Temps réel robuste** : basé sur SignalR, s’adapte automatiquement entre WebSocket, SSE ou LongPolling selon le navigateur.
* **Architecture extensible** : on peut remplacer la simulation par une vraie source GPS / IoT.
* **Gestion des connexions** : savoir qui suit quelle ligne, combien de temps et pouvoir gérer l’accès.
* **Expérience utilisateur riche** : mises à jour instantanées sans rechargement de page.
* **Facilement adaptable** : code clair pour intégrer d’autres métriques (vitesse, retards, ETA).

---

### 6️⃣ Scénarios d’usage en production

* Suivi en temps réel des bus pour les usagers (mobile/web).
* Dashboard pour les opérateurs de flotte avec contrôle dynamique.
* Système de notifications (ex. retards, incidents).
* Extension possible : suivi de plusieurs réseaux de transport, cartes géographiques, intégration avec GPS embarqué.

---

👉 Cette démo sert de base solide pour un projet de suivi de flotte. Elle montre comment structurer un **écosystème temps réel complet** : front (SignalR + JS) et back (Hubs, services de fond, stockage d’état).
