(function () {
    'use strict';

    /**
     * 监听表单加载并自动填充登录信息
     */
    function observeAndFillLoginForm() {
        const formLogin = document.getElementById('formLogin');
        if (formLogin) {
            console.log('表单已就绪，开始填充数据...');
            fillLoginForm(formLogin);
        } else {
            // 使用MutationObserver等待动态加载的表单
            const observer = new MutationObserver((mutations) => {
                mutations.forEach((mutation) => {
                    mutation.addedNodes.forEach((node) => {
                        if (node.id === 'formLogin') {
                            console.log('检测到表单动态加载，开始处理...');
                            observer.disconnect(); // 停止观察
                            fillLoginForm(node);
                        }
                    });
                });
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    }

    /**
     * 填充表单并模拟点击登录按钮
     * param {HTMLElement} form 表单元素
     */
    function fillLoginForm(form) {
        const userNameInput = form.querySelector('#userName');
        const passwordInput = form.querySelector('#password');
        const loginButton = form.querySelector('button[type="submit"]');

        if (userNameInput && passwordInput && loginButton) {
            userNameInput.value = 'K08013'; // 示例用户名
            passwordInput.value = 'jsbbdzz'; // 示例密码
            console.log('用户名和密码已填充。');

            // 等待600毫秒后模拟点击登录按钮，以适应页面可能的异步加载
            setTimeout(() => {
                loginButton.click();
                console.log('已尝试自动点击登录。');
            }, 600);
        } else {
            console.error('表单元素未找到或配置不完整，无法自动填充。');
        }
    }

    // 启动表单监控及填充流程
    observeAndFillLoginForm();

    // 监听Ajax请求完成事件，以便获取登录后的Cookie
    const originalXHR = window.XMLHttpRequest;
    window.XMLHttpRequest = function () {
        const xhr = new originalXHR();
        xhr.addEventListener('readystatechange', function () {
            if (xhr.readyState === 4) { // 请求完成
                window.postMessage({ type: 'ajaxCompleted', url: xhr.responseURL }, '*');
            }
        });
        return xhr;
    };
})();
