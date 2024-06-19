function login() {
    var username = 'US01';
    var password = 'jsbbdzh';
    var buttonId = 'button-1009';

    var usernameInput = document.getElementById('UserName');
    if (usernameInput) {
        usernameInput.value = username;
    }

    var passwordInput = document.getElementById('UserPwd');
    if (passwordInput) {
        passwordInput.value = password;
    }

    var loginButton = document.getElementById(buttonId);
    if (loginButton) {
        var event = new MouseEvent('click', {
            bubbles: true,
            cancelable: true,
            view: window
        });
        loginButton.dispatchEvent(event);
    }

    'use strict';
    //——————————————————————————————————————————————————
    const 下拉小三角选择器 = '#panel-1029-splitter #panel-1029-splitter-collapseEl';
    const 初始检查频率 = 500; // 毫秒

    // 点击元素函数
    const 点击元素 = 元素 => 元素.click();

    // 初始化表格观察者
    let 表格观察者;

    function 初始化表格观察() {
        表格观察者 = new MutationObserver((变更列表, 观察器) => {
            变更列表.forEach(变更 => {
                if (变更.type === 'childList') {
                    const 表格 = document.querySelector('#treeview-1012-record-14');
                    if (表格) {
                        console.log('找到目标表格');
                        观察器.disconnect(); // 找到后停止观察
                        检查并点击行();
                    } else {
                        // 如果表格没找到或消失，继续观察
                        console.log('未找到目标表格，继续观察...');
                    }
                }
            });
        });
        表格观察者.observe(document.body, { childList: true, subtree: true });
    }

    // 立即开始观察表格
    初始化表格观察();

    // 定时检查并点击下拉小三角
    let 定时器 = setInterval(() => {
        const 三角形 = document.querySelector(下拉小三角选择器);
        if (三角形) {
            console.log('找到下拉小三角，执行点击...');
            点击元素(三角形);
            clearInterval(定时器); // 停止定时器
            // 观察逻辑已初始化，无需重复操作
        }
    }, 初始检查频率);


    // 创建观察器的回调函数
    function 观察器回调(mutationsList, observer) {
        mutationsList.forEach(mutation => {
            if (mutation.type === 'childList') {
                mutation.addedNodes.forEach(addedNode => {
                    if (addedNode.matches && addedNode.matches('#ext-element-25 .x-tree-node-text')) {
                        console.log('通过MutationObserver找到目标<div>元素，准备点击');
                        点击元素(addedNode);
                        observer.disconnect(); // 停止观察
                    }
                });
            }
        });
    }

    //——————————————————————————————————————————————————
    function 初始化观察并点击动态或静态元素(目标容器Selector, targetElementSelector) {
        const 目标容器 = document.querySelector(目标容器Selector);
        if (!目标容器) {
            console.error('无法找到监视的目标容器');
            return;
        }

        // 尝试立即查找目标元素，以处理静态情况
        const 目标元素 = document.querySelector(targetElementSelector);
        if (目标元素) {
            console.log('立即找到目标<div>元素，准备点击');
            点击元素(目标元素);
            return; // 如果已找到静态元素，则无需继续观察
        }

        // 如果静态查找失败，设置MutationObserver来监视动态变化
        const 观察配置 = { childList: true, subtree: true };
        const 观察器 = new MutationObserver((mutationsList, observer) => {
            for (const mutation of mutationsList) {
                if (mutation.type === 'childList') {
                    for (const addedNode of mutation.addedNodes) {
                        if (addedNode.nodeType === Node.ELEMENT_NODE && addedNode.matches(targetElementSelector)) {
                            console.log('通过MutationObserver找到目标<div>元素，准备点击');
                            点击元素(addedNode);
                            observer.disconnect(); // 停止观察
                            break;
                        }
                    }
                }
            }
        });

        观察器.observe(目标容器, 观察配置);
    }

    // 修改检查并点击行函数以利用初始化观察函数
    function 检查并点击行() {
        const 表格 = document.getElementById('treeview-1012-record-14');
        if (表格) {
            const 目标行 = 表格.querySelector('tr.x-grid-row[aria-expanded="false"]');
            if (目标行) {
                console.log('找到符合条件的行元素，准备点击展开图标');
                const 展开图标 = 目标行.querySelector('.x-tree-elbow-img.x-tree-elbow-end-plus.x-tree-expander');
                if (展开图标) {
                    展开图标.click();
                    // 确定监视的目标容器和目标元素选择器
                    初始化观察并点击动态或静态元素('#treeview-1012-record-14', '#ext-element-25 .x-tree-node-text');
                } else {
                    console.error('展开图标未找到');
                }
            } else {
                console.log('未找到aria-expanded为"false"的<tr>元素');
            }
        } else {
            console.error('表格元素未找到');
        }
    }
    //——————————————————————————————————————————————————
}

// 调用login函数进行登录操作
login();
