// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Calculator logic
(function () {
  const display = document.getElementById('calc-display');
  const keypad = document.querySelector('.calculator-grid');
  if (!display || !keypad) return;

  let current = '0';
  let previous = null;
  let operator = null;
  let justEvaluated = false;

  function setResultHighlight(isResult) {
    if (isResult) {
      display.classList.add('calculator-display-result');
    } else {
      display.classList.remove('calculator-display-result');
    }
  }

  function updateDisplay() {
    display.value = current;
  }

  function inputDigit(d) {
    if (justEvaluated) { current = '0'; justEvaluated = false; }
    setResultHighlight(false);
    if (current === '0' && d !== '.') {
      current = d;
    } else if (d === '.' && current.includes('.')) {
      return;
    } else {
      current += d;
    }
    updateDisplay();
  }

  function setOperator(op) {
    if (operator && !justEvaluated) {
      evaluate();
    } else {
      previous = parseFloat(current);
    }
    setResultHighlight(false);
    operator = op;
    justEvaluated = false;
    current = '0';
  }

  function clearAll() {
    setResultHighlight(false);
    current = '0';
    previous = null;
    operator = null;
    justEvaluated = false;
    updateDisplay();
  }

  function backspace() {
    if (justEvaluated) { return; }
    setResultHighlight(false);
    if (current.length <= 1 || (current.length === 2 && current.startsWith('-'))) {
      current = '0';
    } else {
      current = current.slice(0, -1);
    }
    updateDisplay();
  }

  function toggleSign() {
    if (current === '0') return;
    setResultHighlight(false);
    current = current.startsWith('-') ? current.slice(1) : '-' + current;
    updateDisplay();
  }

  function toPercent() {
    setResultHighlight(false);
    const num = parseFloat(current || '0');
    current = (num / 100).toString();
    updateDisplay();
  }

  async function evaluate() {
    if (operator === null || previous === null) return;
    const a = previous;
    const b = parseFloat(current);
    
    // Отправляем запрос на сервер для расчета
    try {
      const formData = new FormData();
      formData.append('Number1', a.toString());
      formData.append('Number2', b.toString());
      formData.append('Operation', operator);

      const response = await fetch('/Home/Calculator', {
        method: 'POST',
        headers: {
          'X-Requested-With': 'XMLHttpRequest'
        },
        body: formData
      });

      if (!response.ok) {
        throw new Error('Ошибка сервера');
      }

      const data = await response.json();
      
      if (data.errorMessage) {
        current = data.errorMessage;
      } else if (data.result !== null && data.result !== undefined) {
        current = String(round10(data.result, 10));
      } else {
        current = 'Ошибка';
      }
    } catch (error) {
      console.error('Ошибка при отправке запроса:', error);
      // Fallback на клиентский расчет в случае ошибки
      let res = a;
      switch (operator) {
        case '+': res = a + b; break;
        case '-': res = a - b; break;
        case '*': res = a * b; break;
        case '/': res = b === 0 ? NaN : a / b; break;
      }
      current = String(Number.isFinite(res) ? round10(res, 10) : 'Ошибка');
    }
    
    previous = null;
    operator = null;
    justEvaluated = true;
    setResultHighlight(true);
    updateDisplay();
  }

  function round10(value, digits) {
    const p = Math.pow(10, digits);
    return Math.round((value + Number.EPSILON) * p) / p;
  }

  keypad.addEventListener('click', (e) => {
    const btn = e.target.closest('button');
    if (!btn) return;
    const value = btn.getAttribute('data-value');
    const action = btn.getAttribute('data-action');
    if (value !== null) {
      if (['+', '-', '*', '/'].includes(value)) {
        setOperator(value);
      } else {
        inputDigit(value);
      }
    } else if (action) {
      handleAction(action);
    }
  });

  function handleAction(action) {
    switch (action) {
      case 'clear': clearAll(); break;
      case 'back': backspace(); break;
      case 'sign': toggleSign(); break;
      case 'percent': toPercent(); break;
      case 'equals': evaluate(); break;
    }
  }

  document.addEventListener('keydown', (e) => {
    const key = e.key;
    if (/^[0-9]$/.test(key)) { inputDigit(key); return; }
    if (key === '.') { inputDigit('.'); return; }
    if (key === '+' || key === '-' || key === '*' || key === '/') { setOperator(key); return; }
    if (key === 'Enter' || key === '=') { e.preventDefault(); evaluate(); return; }
    if (key === 'Backspace') { backspace(); return; }
    if (key.toLowerCase() === 'c') { clearAll(); return; }
    if (key === '%') { toPercent(); return; }
  });
})();
