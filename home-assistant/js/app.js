// CONFIGURAÇÕES GLOBAIS
// Altere para 'false' para usar o IP real do ESP32.
const USAR_MOCK = false; 
// Endereço IP do seu dispositivo ESP32.D
const IP_ESP32 = "http://192.168.0.100"; 

/**
 * Função genérica para buscar dados de um endpoint (da API no ESP32).
 * @param {string} endpoint - O caminho do endpoint na API (ex: "/api/clima").
 * @param {function} callbackSucesso - Função a ser executada se a busca for bem-sucedida. Ela recebe os dados (JSON) como argumento.
 * @param {function} [callbackErro] - (Opcional) Função a ser executada em caso de erro.
 */
async function buscarDados(endpoint, callbackSucesso, callbackErro) {
    const url = `${IP_ESP32}${endpoint}`;
    console.log(`Buscando dados de: ${url}`);

    try {
        let dados;
        if (USAR_MOCK) {
            // Se o MOCK estiver ativo, simula a resposta da API.
            await new Promise(r => setTimeout(r, 500)); // Simula atraso de rede
            
            // Simula respostas diferentes para cada endpoint
            if (endpoint.includes("clima")) {
                dados = { "temperatura": 27.50, "umidade": 62.00 };
            } else if (endpoint.includes("led")) {
                dados = { "estado": "ON" };
            } else {
                dados = {};
            }
        } else {
            // Busca os dados reais do ESP32.
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`Erro HTTP: ${response.status}`);
            }
            dados = await response.json();
        }
        // Executa a função de sucesso, passando os dados.
        if (callbackSucesso) {
            callbackSucesso(dados);
        }

    } catch (erro) {
        console.error(`Erro ao buscar dados do endpoint '${endpoint}':`, erro);
        // Executa a função de erro, se ela foi fornecida.
        if (callbackErro) {
            callbackErro(erro);
        }
    }
}


/**
 * Atualiza os elementos do dashboard com os dados de clima (temperatura e umidade).
 */
function atualizarCardClima() {
    const endpoint = "/api/clima";

    buscarDados(endpoint, 
        // Função de sucesso:
        (dados) => {
            // Atualiza os elementos na tela (DOM) com os valores recebidos.
            const tempEl = document.getElementById('temp-display');
            const umidEl = document.getElementById('umid-display');

            if (tempEl) tempEl.innerText = dados.temperatura.toFixed(1);
            if (umidEl) umidEl.innerText = dados.umidade.toFixed(1);
        },
        // Função de erro:
        () => {
            const tempEl = document.getElementById('temp-display');
            const umidEl = document.getElementById('umid-display');
            if (tempEl) tempEl.innerText = "--";
            if (umidEl) umidEl.innerText = "--";
        }
    );
}


/*
// ---- EXEMPLO PARA O FUTURO: ATIVIDADE DO LED ----
// Função para atualizar o status do LED.
function atualizarCardLed() {
    const endpoint = "/api/led";

    buscarDados(endpoint, 
        (dados) => {
            const ledStatusEl = document.getElementById('led-status-display');
            if (ledStatusEl) {
                ledStatusEl.innerText = dados.estado;
            }
        },
        () => {
            const ledStatusEl = document.getElementById('led-status-display');
            if (ledStatusEl) {
                ledStatusEl.innerText = "N/A";
            }
        }
    );
}
*/


// --- INICIALIZAÇÃO ---
// Quando a página carregar, chama as funções para atualizar os cards.
document.addEventListener('DOMContentLoaded', () => {
    atualizarCardClima();
    // Futuramente, você pode adicionar outras funções aqui:
    // atualizarCardLed();

    // (Opcional) Define um intervalo para atualizar os dados automaticamente.
    setInterval(atualizarCardClima, 3000); 
    // setInterval(atualizarCardLed, 5000); 
});
