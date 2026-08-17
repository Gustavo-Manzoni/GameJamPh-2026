# Sistema de Fila de Seguidores com Game Feel

Sistema topdown 2D: pessoas paradas no mapa entram numa fila ao serem tocadas pela
área de detecção do jogador, e cada uma segue exatamente o caminho de quem está na
sua frente (jogador ou outra pessoa), com espaçamento, mola e squash & stretch.

## O que cada script faz

| Script | Função |
|---|---|
| `Spring.cs` | Mola (spring-damper) genérica em float e Vector2 — base de todo o juice |
| `SquashStretch.cs` | Aplica squash & stretch elástico num sprite via mola |
| `CameraShake.cs` | Shake de câmera por "trauma" (decai sozinho, não-linear) |
| `PositionRecorder.cs` | Grava o histórico de posições — é o que permite a fila seguir o caminho exato |
| `FollowerPerson.cs` | Estado Idle/Following de cada pessoa + movimento na fila + juice |
| `FollowChainManager.cs` | Decide quem cada pessoa nova vai seguir (fila) |
| `PlayerDetectionArea.cs` | Área ao redor do jogador que recruta pessoas paradas |
| `PlayerController.cs` | Movimento do jogador + juice (stretch, lean, bob) |
| `ChainLineRenderer.cs` | (Opcional) linha visual conectando a fila toda |

## Como técnica de "fila que segue o caminho" funciona

Cada `PositionRecorder` grava um rastro de pontos por onde o objeto passou. Quando
uma pessoa entra na fila, ela não segue a posição atual de quem está na frente —
ela busca o ponto do rastro que fica a `spacing` unidades de distância *percorrida*
para trás. Isso é o que cria o efeito de fila serpenteando pelas curvas, igual
Pikmin/Katamari, em vez de todo mundo cortando caminho reto.

## Montagem na Hierarchy

**1. Player**
- `Rigidbody2D` (Dynamic, Gravity Scale = 0, Freeze Rotation Z marcado)
- Um `Collider2D` (corpo físico normal)
- Componentes: `PlayerController`, `PositionRecorder`
- Filho **"Visual"** com o `SpriteRenderer` → arraste esse filho no campo `visual` do
  `PlayerController`
- Adicione `SquashStretch` no próprio Player (ou no Visual), com:
  - `visual` = o filho "Visual"
  - `reactToVelocity` = ✔️
  - `body` = o Rigidbody2D do Player
  - arraste esse componente no campo `squashStretch` do `PlayerController`
- Filho **"DetectionArea"** com `Collider2D` (Circle, por exemplo) marcado `Is Trigger`,
  raio = o quanto você quer que a "área ao redor de você" alcance → componente
  `PlayerDetectionArea`

**2. ChainManager** (objeto vazio na cena)
- Componente `FollowChainManager`
- `playerRecorder` = arraste o `PositionRecorder` do Player
- `spacing` = distância entre pessoas na fila (0.6 é um bom ponto de partida)

**3. Main Camera**
- Adicione `CameraShake`

**4. Prefab "Person"** (uma pra cada pessoa do mapa)
- `Collider2D` normal (não precisa de Rigidbody2D — quem detecta a colisão é o Player)
- Componentes: `FollowerPerson` (o `PositionRecorder` é adicionado automaticamente)
- Filho **"Visual"** com `SpriteRenderer` → arraste no campo `visual` do `FollowerPerson`
- Adicione `SquashStretch` nesse Person (com `visual` apontando pro filho, `reactToVelocity`
  desmarcado) → arraste no campo `squashStretch` do `FollowerPerson`

**5. (Opcional) Linha da fila**
- Objeto com `LineRenderer` + `ChainLineRenderer`, arraste o `chainManager` e o `player`

## Ajustando o "feel"

- **Mais elástico/exagerado**: diminua `damping` em relação a `stiffness` nas molas
  (ex.: stiffness 250 / damping 10 → bounce visível). Cuidado pra não deixar oscilando
  pra sempre — se parecer "vidrado", aumente o damping.
- **Mais responsivo/rígido**: aumente `stiffness` e `damping` juntos, mantendo a
  proporção.
- **Fila mais "grudada"**: aumente `positionStiffness` do `FollowerPerson` e diminua
  o `spacing` do `FollowChainManager`.
- **Fila mais "líquida"/atrasada**: diminua `positionStiffness` do `FollowerPerson`.

## Ganchos prontos pra som/partículas

- `PlayerController` não expõe eventos públicos ainda — se quiser, adicione um
  `System.Action OnStartMoving/OnStopMoving` no `FixedUpdate` (onde já detectamos
  `isMoving`) pra disparar poeira nos pés, por exemplo.
- `FollowChainManager.OnFollowerJoined` já é um evento público — assine ele em
  qualquer script pra tocar som/partícula quando alguém entra na fila.

## Ideias de extensão

- Pessoas com pequena IA de "wander" (vaguear) enquanto Idle, usando o próprio
  `SquashStretch` pra um "pulo" de curiosidade quando o jogador se aproxima.
- Botão pra "soltar" a fila inteira (`FollowChainManager.RemoveFollower` num loop).
- Limitar o tamanho máximo da fila.
- Trocar a `LineRenderer` por sprites de "corda" entre cada pessoa.
