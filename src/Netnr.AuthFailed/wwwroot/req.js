let nrWeb = {
    init: () => {
        nrWeb.reqAuthorizationInfo();

        nrWeb.domBtnLogin.addEventListener("click", async function () {
            this.disabled = true;

            try {
                let fd = new FormData();
                fd.append("userAccount", "netnr");
                fd.append("userPassword", "123456");

                let resp = await fetch("/Home/LoginPost", { method: "POST", body: fd });
                let text = await resp.text();
                nrWeb.token = text;
                nrWeb.addLog(resp, text);
            } catch (e) {
                console.debug(e);
                nrWeb.addLog(`CATCH! ${e}`);
            }

            this.disabled = false;
        });

        nrWeb.domBtnLogout.addEventListener("click", async function () {
            nrWeb.token = null;
            alert('Cleaned JWT Token\n\nPlease clear cookie ".auth" in the console');
        });

        nrWeb.domBtnPause.addEventListener("click", async function () {
            nrWeb.isPause = !nrWeb.isPause;
            console.debug(`isPause: ${nrWeb.isPause}`);
        });

        nrWeb.domBtnCpu.addEventListener("click", async function () {
            let cpu = prompt("Please enter a percentage (1-100) to simulate CPU usage, enter 0 to stop", "20");
            cpu = parseInt(cpu) || 0;
            let resp = await fetch(`/Home/ConsumeCPU?cpu=${cpu}`);
            let text = await resp.text();
            nrWeb.addLog(resp, text);
        });
    },
    domTxt: document.querySelector("textarea"),
    domBtnPause: document.querySelector(".btn-pause"),
    domBtnLogin: document.querySelector(".btn-login"),
    domBtnLogout: document.querySelector(".btn-logout"),
    domCkJwt: document.querySelector(".ck-jwt"),
    domBtnCpu: document.querySelector(".btn-cpu"),
    isPause: false,
    token: null,
    addLog: (resp, body) => {
        console.debug(resp, body);
        let authType = resp.headers.get("x-auth-type");
        let msg = `url: ${resp.url}\nok: ${resp.ok}, status: ${resp.status}, statusText: ${resp.statusText}, auth-type: ${authType}, body: ${body}`;
        nrWeb.domTxt.value = `${nrWeb.domTxt.value}[${new Date().toLocaleString()}]: ${msg}\n\n`;

        nrWeb.domTxt.scrollBy(0, Number.MAX_SAFE_INTEGER);
    },
    sleep: (time) => new Promise(resolve => setTimeout(() => resolve(), time || 1000)),
    reqAuthorizationInfo: async () => {
        if (!nrWeb.isPause) {
            try {
                let options = nrWeb.domCkJwt.checked
                    ? { headers: { Authorization: `Bearer ${nrWeb.token}` } }
                    : null;

                let resp = await fetch("/Home/AuthorizationInfoGet", options);
                let text = await resp.text();
                nrWeb.addLog(resp, text);
            } catch (e) {
                console.debug(e);
                nrWeb.addLog(`CATCH! ${e}`);
            }
        }

        await nrWeb.sleep(2000);
        await nrWeb.reqAuthorizationInfo();
    }
}

nrWeb.init();