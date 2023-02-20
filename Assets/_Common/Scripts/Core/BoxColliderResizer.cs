using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxColliderResizer : CMonoBehaviour
{
    private void Resize(){
        RectTransform itemRectTransform = this.gameObject.GetComponent<RectTransform>();
        // strech box collider
        BoxCollider2D itemBoxCollider2D =  itemRectTransform.gameObject.GetComponent<BoxCollider2D>();
        if (itemBoxCollider2D != null)
        {
            Image image = itemRectTransform.gameObject.GetComponent<Image>();
            if (image.preserveAspect) {
                
                int originalW = (int)(image.sprite.rect.width);
                int originalH = (int)(image.sprite.rect.height);

                float currentW = image.rectTransform.rect.width;
                float currentH = image.rectTransform.rect.height;
                
                float ratio = Mathf.Min (currentW / originalW, currentH / originalH);

                float newW = image.sprite.rect.width * ratio;
                float newH = image.sprite.rect.height * ratio;

                itemBoxCollider2D.size = new Vector2 (newW, newH);
            } else {
                itemBoxCollider2D.size = image.rectTransform.rect.size;
            }
        }
    }

    void OnEnable() {
        CallAfterFixedUpdate( () => CallNextFrame(Resize));
    }

    void Update(){
        Resize();
    }
}
