(function () {
    'use strict';

    (function () {
        'use strict';

        /**
     * 监听表单加载并自动填充登录信息
     */
        function observeAndFillLoginForm() {
            const formLogin = document.getElementById('wrapper');
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
     * @param {HTMLElement} form 表单元素
     */
        function fillLoginForm(form) {
            // 更新的查询选择器
            const userNameInput = form.querySelector('.bi-abs input[type="text"]');
            const passwordInput = form.querySelector('.bi-abs input[type="password"]');
            const checkboxToClick = form.querySelector('.bi-basic-button.cursor-pointer.bi-checkbox');
            const loginButton = form.querySelector('.login-button');

            if (userNameInput && passwordInput && checkboxToClick && loginButton) {
                userNameInput.value = '100898'; // 示例用户名
                passwordInput.value = 'abc,123'; // 示例密码
                console.log('用户名和密码已填充。');

                // 模拟点击复选框
                setTimeout(() => {
                    checkboxToClick.click();
                    console.log('复选框已点击。');

                    // 稍作延迟后点击登录按钮，以确保页面有足够时间处理复选框的点击事件
                    setTimeout(() => {
                        loginButton.click();
                        console.log('已尝试自动点击登录。');
                    }, 200);
                }, 200);
            } else {
                console.error('表单元素未找到或配置不完整，无法自动填充。');
            }
        }

        // 启动表单监控及填充流程
        observeAndFillLoginForm();
    })();

})();