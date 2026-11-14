// Externí fotografie mohou být nedostupné; rozložení karet zůstane zachované.
document.querySelectorAll('img[data-recipe-image]').forEach(image => {
    const fallback = () => {
        image.removeEventListener('error', fallback);
        image.src = image.dataset.recipeImage;
    };
    image.addEventListener('error', fallback);
    if (image.complete && image.naturalWidth === 0) fallback();
});
