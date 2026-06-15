

# 📡 Luma API — Documentação (Lacuna Dev Challenge)

**Base URL:**

```
https://luma.lacuna.cc/
```

**Content-Type padrão:**

```
application/json
```

**Autenticação (quando aplicável):**

```
Authorization: Bearer {accessToken}
```

---

# 📦 Base Response (padrão de todas as respostas)

```json
{
  "code": "string",
  "message": "string (optional)"
}
```

---

# 1. 🚀 Iniciar Contexto

## 📌 POST `/api/start`

### Request

```json
{
  "username": "string",
  "email": "string"
}
```

### Response

```json
{
  "accessToken": "string (optional)",
  "code": "Success | Error",
  "message": "string (optional)"
}
```

### Regras importantes

* Retorna `accessToken`
* Token válido por **2 minutos**
* Usado em todas as rotas autenticadas
* Header obrigatório:

```
Authorization: Bearer {accessToken}
```

---

# 2. 🛰️ Listar Sondas

## 📌 GET `/api/probe`

### Headers

```
Authorization: Bearer {accessToken}
```

### Response

```json
{
  "probes": [
    {
      "id": "string",
      "name": "string",
      "encoding": "Iso8601 | Ticks | TicksBinary | TicksBinaryBigEndian"
    }
  ],
  "code": "Success | Error | Unauthorized",
  "message": "string (optional)"
}
```

---

# 3. ⏱️ Codificação de Timestamp

As sondas usam diferentes formatos para representar `Ticks (Int64)`:

## Tipos suportados

* `Iso8601`
* `Ticks`
* `TicksBinary`
* `TicksBinaryBigEndian`

---

## Exemplo (valor base: 638213938476003807)

| Encoding             | Exemplo                               |
| -------------------- | ------------------------------------- |
| Iso8601              | `"2023-06-03T12:57:27.6003807+00:00"` |
| Ticks                | `"638213938476003807"`                |
| TicksBinary          | `"37GQFTJk2wg="`                      |
| TicksBinaryBigEndian | `"CNtkMhWQsd8="`                      |

---

# 4. ⏳ Sincronização de Relógio

## 📌 POST `/api/probe/{id}/sync`

### URL Params

```
id = Probe ID
```

### Headers

```
Authorization: Bearer {accessToken}
```

---

### Response

```json
{
  "t1": "string (encoded timestamp)",
  "t2": "string (encoded timestamp)",
  "code": "Success | Error | Unauthorized",
  "message": "string (optional)"
}
```

---

## 🧠 Algoritmo de sincronização

Você deve capturar:

```
t0 = antes da requisição
t3 = depois da resposta
```

Servidor retorna:

```
t1 = após receber request
t2 = antes de enviar response
```

---

## 📐 Fórmulas

### Offset de tempo (θ)

```
θ = ((t1 - t0) + (t2 - t3)) / 2
```

---

### Round trip (σ)

```
σ = (t3 - t0) - (t2 - t1)
```

---

## 🔁 Regras

* Pode repetir chamadas até melhorar precisão
* Somar offsets sucessivos
* Considerar sincronizado quando:

```
θ < 5ms
```

---

# 5. 💼 Jobs (Trabalhos)

## 📌 POST `/api/job/take`

### Headers

```
Authorization: Bearer {accessToken}
```

### Response

```json
{
  "job": {
    "id": "string",
    "probeName": "string"
  },
  "code": "Success | Error | Unauthorized",
  "message": "string (optional)"
}
```

---

## 📌 POST `/api/job/{id}/check`

### URL Params

```
id = Job ID
```

### Headers

```
Authorization: Bearer {accessToken}
```

---

### Request

```json
{
  "probeNow": "string (encoded timestamp)",
  "roundTrip": "number"
}
```

---

### Response

```json
{
  "code": "Success | Done | Fail | Error | Unauthorized",
  "message": "string (optional)"
}
```

---

## 🧠 Regras de execução

* Continuar pegando jobs até `job = null`
* Cada job deve ser validado com:

    * tempo sincronizado da sonda
    * round trip calculado
* Quando retornar:

```
Done → sucesso final 🎉
```

---

# 6. ⚠️ Regras Globais

## ❌ Fail em qualquer momento

* Reiniciar todo o fluxo

---

## ⛔ Unauthorized

* Token expirado
* Gerar novo `/api/start`

---

# 7. 🌌 Modo Avançado (Level 2)

## 📌 POST `/api/start/2`

Mesmas regras de `/api/start`, com diferenças:

---

## ⚠️ Nova regra: Probe Unreachable

Se receber:

```
ProbeUnreachable
```

👉 Esperar **5 segundos** e tentar novamente

---

## 🌍 Time Dilation

Algumas sondas podem conter:

```json
timeDilationFactor: number
```

### Regra:

O tempo terrestre passa mais rápido.

👉 Ajustar cálculos de tempo com esse fator.

---

# 🎯 Fluxo Geral do Sistema

```text
1. POST /api/start → accessToken

2. GET /api/probe → lista sondas

3. sincronizar relógios:
   POST /api/probe/{id}/sync

4. pegar jobs:
   POST /api/job/take

5. validar jobs:
   POST /api/job/{id}/check

6. repetir até Done
```

---
